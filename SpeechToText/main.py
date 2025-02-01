#!/usr/bin/env -P /usr/bin:/usr/local/bin python3 -B
# coding: utf-8

#
#  main.py
#  SpeechToTextController
#  Created by Ingenuity i/o on 2025/01/08
#
#  Responsable de la conversion parole vers texte
# To run : python ./main.py
# Parameters : port, agent_name, device below
# renseigner également le modèle vosk

import sys
import os
import wave
import json
import vosk
import pyaudio
import ingescape as igs
import threading
import time

port = 5140
agent_name = "SpeechToTextController"
device = "Wi-Fi"

precedent_partiel_text = []

def Elementcreated_callback(sender_agent_name, sender_agent_uuid, service_name, tuple_args, token, my_data):
    Elementid = tuple_args[0]
    #if token == "easter_egg":


def on_agent_event_callback(event, uuid, name, event_data, my_data):
    if name == "Whiteboard":
        if event == igs.AGENT_KNOWS_US:
            igs.service_call("Whiteboard", "clear", None, "")
            time.sleep(1)
            igs.service_call("Whiteboard", "chat", ("Agent SpeechToText de connecté"), "")
        elif event == igs.AGENT_EXITED:
            igs.service_call("Whiteboard", "chat", ("Agent SpeechToText déconnecté"), "")
            print ("whiteboard exited")


# Attention, si on ne fait que analyse partiel on peut rater des mots, il faut donc analyser le texte complet également

def affichage_text(id, text):
    '''
    Cette fonction est appelée pour afficher le texte sur le tableau blanc
    '''
    igs.service_call("Whiteboard", "setStringProperty", (id, text), "")

# On remet le texte partiel à zéro
precedent_partiel_text = []

def recognise_output(text, isPartial=True):
    '''
    This function is called to see if the output contains some specific keywords 
    and then set the directionOutput accordingly
    '''
    global precedent_partiel_text

    # split the string into words
    text = text.split(" ")
    text_mem = text

    # On ne regarde dans le texte partiel que les mots qui n'ont pas déjà été reconnus au préalable
    if isPartial:
        if (precedent_partiel_text != []):
            diff = len(text) - len(precedent_partiel_text)
        else:
            diff = 0

        if (diff > 0):
            # On retire les diff premiers mots de text
            text = text[diff:]    

    for c in text:
        if c in ["haut", "oh", "ho", "au", "aux", "eau", "hauts", "eaux", "o", "ô"]:
            igs.output_set_int("directionOutput", 0)
            affichage_text(0, "Haut")
            print("Haut reconnu")
            continue
        if c in ["droite", "droites", "droit", "trois", "troie", "droits"]:
            igs.output_set_int("directionOutput", 1)
            affichage_text(0, "Droite")
            print("Droite reconnue")
            continue
        if c in ["bas", "ba", "bât", "bat", "bât", "bats", "bras", "bah", "ben"]:
            igs.output_set_int("directionOutput", 2)
            affichage_text(0, "Bas")
            print("Bas reconnu")   
            continue
        if c in ["gauche", "gouache", "gouaches", "gauches"]:
            igs.output_set_int("directionOutput", 3)
            affichage_text(0, "Gauche")
            print("Gauche reconnue")
            continue
        if c in ["clear", "clair", "claire", "clairs", "claires", "lire", "effacer", "effacé", "effacée", "effacés", "effacées"]:
            print("Clear reconnu")
            affichage_text(0, "Clear")
            igs.service_call("Whiteboard", "clear", (), "")
            continue
        if c in ["pokémon"]:
            print("Easter egg")
            affichage_text(0, "Easter egg !")
            igs.service_call("Whiteboard", "addImageFromUrl", ("data:image/jpeg;base64,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"
            , 0, 0), "easter_egg")
            continue


    if isPartial:
        precedent_partiel_text = text_mem


def start_recognition():
    # Params
    framerate = 16000
    channels = 1

    # Initialiser le modèle Vosk
    model_path = "vosk-model-small-fr-0.22"
    if not os.path.exists(model_path):
        print(f"Le modèle {model_path} n'existe pas. Veuillez le télécharger.")
        exit(1)

    model = vosk.Model(model_path)

    # Configurer PyAudio
    p = pyaudio.PyAudio()
    stream = p.open(format=pyaudio.paInt16, channels=channels, rate=framerate, input=True, frames_per_buffer=int(framerate * 0.4))
    stream.start_stream()

    # Initialiser le reconnaisseur Vosk
    recognizer = vosk.KaldiRecognizer(model, 16000)

    # Boucle de reconnaissance en temps réel
    print("Vous pouvez parler maintenant...")
    while True:
        data = stream.read(16000, exception_on_overflow=False)
        if len(data) == 0:
            break
        if recognizer.AcceptWaveform(data):
            result = recognizer.Result()
            result_dict = json.loads(result)
            if (result_dict['text'] != ""):
                print(f"Vous avez dit : {result_dict['text']}")
                igs.output_set_string("textOutput", result_dict['text'])
                recognise_output(result_dict['text'], False)
            # Arrêter la boucle si un mot spécifique est détecté
            # if "roger" in result_dict['text']:
            #     break
        else:
            # Résultat partiel (+ rapide)
            partial_result = recognizer.PartialResult()
            partial_result_dict = json.loads(partial_result)
            if (partial_result_dict['partial'] != ""):
                print(f"Partiel : {partial_result_dict['partial']}")
                #recognise_output(partial_result_dict['partial'])

    # Fermer le flux audio
    stream.stop_stream()
    stream.close()
    p.terminate()

if __name__ == "__main__":
    # if len(sys.argv) < 4:
    #     print("usage: python3 main.py agent_name network_device port")
    #     devices = igs.net_devices_list()
    #     print("Please restart with one of these devices as network_device argument:")
    #     for device in devices:
    #         print(f" {device}")
    #     exit(0)

    igs.agent_set_name(agent_name)
    igs.definition_set_description("""<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0//EN\" \"http://www.w3.org/TR/REC-html40/strict.dtd\">\n<html><head><meta name=\"qrichtext\" content=\"1\" /><meta charset=\"utf-8\" /><style type=\"text/css\">\np, li { white-space: pre-wrap; }\nhr { height: 1px; border-width: 0; }\nli.unchecked::marker { content: \"\\2610\"; }\nli.checked::marker { content: \"\\2612\"; }\n</style></head><body style=\" font-family:'Asap'; font-size:13px; font-weight:400; font-style:normal;\">\n<p style=\" margin-top:0px; margin-bottom:0px; margin-left:0px; margin-right:0px; -qt-block-indent:0; text-indent:0px;\"><span style=\" font-size:13px;\">Responsable de la conversion parole vers texte</span></p></body></html>""")
    igs.definition_set_class("SpeechToTextController")
    igs.log_set_console(True)
    igs.log_set_file(True, None)
    igs.set_command_line(sys.executable + " " + " ".join(sys.argv))

    igs.debug(f"Ingescape version: {igs.version()} (protocol v{igs.protocol()})")

    igs.output_create("textOutput", igs.STRING_T, None)
    igs.output_create("directionOutput", igs.INTEGER_T, 0)

    igs.service_init("elementCreated", Elementcreated_callback, None)
    igs.service_arg_add("elementCreated", "elementId", igs.INTEGER_T)

    igs.observe_agent_events(on_agent_event_callback, None)

    igs.start_with_device(device, port)

    # Start recognition in a separate thread
    recognition_thread = threading.Thread(target=start_recognition)
    recognition_thread.daemon = True
    recognition_thread.start()

    # Main loop to keep the agent running
    try:
        while igs.is_started():
            time.sleep(1)
    except KeyboardInterrupt:
        print("Interrupted by user")
    finally:
        igs.stop()
