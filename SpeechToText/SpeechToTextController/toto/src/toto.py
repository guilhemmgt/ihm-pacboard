#!/usr/bin/env -P /usr/bin:/usr/local/bin python3 -B
# coding: utf-8

#
#  toto.py
#  toto
#  Created by Ingenuity i/o on 2025/01/10
#
# "no description"
#
import ingescape as igs


class Singleton(type):
    _instances = {}
    def __call__(cls, *args, **kwargs):
        if cls not in cls._instances:
            cls._instances[cls] = super(Singleton, cls).__call__(*args, **kwargs)
        return cls._instances[cls]


class toto(metaclass=Singleton):
    def __init__(self):
        pass
        pass


    # services
    def Elementcreated(self, sender_agent_name, sender_agent_uuid, Elementid):
        pass
        # add code here if needed


