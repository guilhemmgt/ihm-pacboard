import os
import wave
import json
import vosk
import pyaudio

# Params
framerate = 16000
channels = 1
sampwidth = 2

# Initialiser le modèle Vosk
model_path = "vosk-model-small-fr-0.22"
if not os.path.exists(model_path):
    print(f"Le modèle {model_path} n'existe pas. Veuillez le télécharger.")
    exit(1)

model = vosk.Model(model_path)

# Configurer PyAudio
p = pyaudio.PyAudio()
stream = p.open(format=pyaudio.paInt16, channels=1, rate=framerate, input=True, frames_per_buffer=int(framerate * 0.4))
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
        print(f"Vous avez dit : {result_dict['text']}")

        # Arrêter la boucle si un mot spécifique est détecté
        # if "roger" in result_dict['text']:
        #     break
    else:
        # Résultat partiel (+ rapide)
        partial_result = recognizer.PartialResult()
        partial_result_dict = json.loads(partial_result)
        print(f"Partiel : {partial_result_dict['partial']}")

# Fermer le flux audio
stream.stop_stream()
stream.close()
p.terminate()
