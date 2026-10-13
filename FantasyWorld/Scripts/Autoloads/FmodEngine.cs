using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class FmodEngine : Node
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string dllToLoad);

    class Implementation {
        public Implementation()
        {
            FmodEngine.ErrorCheck(FMOD.Studio.System.create(out studioSystem));
            FmodEngine.ErrorCheck(studioSystem.initialize(32, FMOD.Studio.INITFLAGS.LIVEUPDATE, FMOD.INITFLAGS.PROFILE_ENABLE, IntPtr.Zero));

            FmodEngine.ErrorCheck(studioSystem.getCoreSystem(out system));
        }

        ~Implementation()
        {
            FmodEngine.ErrorCheck(studioSystem.unloadAll());
            FmodEngine.ErrorCheck(studioSystem.release());
        }

        public void Update()
        {
            Dictionary<int, FMOD.Channel> stoppedChannels = new Dictionary<int, FMOD.Channel>();
            foreach (var current in channelMap)
            {
                bool isPlaying = false;
                current.Value.isPlaying(out isPlaying);
                if (!isPlaying)
                {
                    stoppedChannels.Add(current.Key, current.Value);
                }
            }
            foreach (var current in stoppedChannels)
            {
                channelMap.Remove(current.Key);
            }
            FmodEngine.ErrorCheck(studioSystem.update());
        }

        public FMOD.Studio.System studioSystem;
        public FMOD.System system;

        public int nextChannelId;

        public Dictionary<string, FMOD.Sound> soundMap = new Dictionary<string, FMOD.Sound>();
        public Dictionary<int, FMOD.Channel> channelMap = new Dictionary<int, FMOD.Channel>();
        public Dictionary<string, FMOD.Studio.EventInstance> eventMap = new Dictionary<string, FMOD.Studio.EventInstance>();
        public Dictionary<string, FMOD.Studio.Bank> bankMap = new Dictionary<string, FMOD.Studio.Bank>();
    }

    static Implementation m_implementation = null;
    
    public override void _Process(float delta)
    {
        Update();
    }

    public static void Init()
    {
        LoadLibrary(System.IO.Path.GetFullPath("fmod.dll"));
        LoadLibrary(System.IO.Path.GetFullPath("fmodstudio.dll"));
        LoadLibrary(System.IO.Path.GetFullPath("fsbank.dll"));
        LoadLibrary(System.IO.Path.GetFullPath("libfsbvorbis64.dll"));

        m_implementation = new Implementation();
    }

    public static void Update()
    {
        m_implementation.Update();
    }

    public static void ShutDown()
    {
        m_implementation = null;
    }

    public static int ErrorCheck(FMOD.RESULT result)
    {
        if (result != FMOD.RESULT.OK)
        {
            GD.Print("FMOD error! (", result, ")\n ", FMOD.Error.String(result));
            return 1;
        }
        return 0;
    }

    public static void LoadSound(string soundName, bool b3d, bool looping, bool stream)
    {
        bool found = m_implementation.soundMap.ContainsKey(soundName);
        if (found)
            return;

        FMOD.MODE mode = FMOD.MODE.DEFAULT;
        mode |= b3d ? FMOD.MODE._3D : FMOD.MODE._2D;
        mode |= looping ? FMOD.MODE.LOOP_NORMAL : FMOD.MODE.LOOP_OFF;
        mode |= stream ? FMOD.MODE.CREATESTREAM : FMOD.MODE.CREATECOMPRESSEDSAMPLE;

        FMOD.Sound sound;
        FMOD.RESULT result = m_implementation.system.createSound(soundName, mode, out sound);
        ErrorCheck(result);
        if (result == FMOD.RESULT.OK)
        {
            m_implementation.soundMap[soundName] = sound;
        }
    }

    public static void UnloadSound(string soundName)
    {
        foreach (var pair in m_implementation.soundMap)
        {
            if (pair.Key == soundName)
            {
                ErrorCheck(pair.Value.release());
                m_implementation.soundMap.Remove(pair.Key);
                break;
            }
        }
    }

    public static int PlaySounds(string soundName, Vector3 position, float volumeDB)
    {
        int channelId = m_implementation.nextChannelId++;
        FMOD.Sound sound;
        bool found = m_implementation.soundMap.TryGetValue(soundName, out sound);
        if (!found)
        {
            LoadSound(soundName, true, false, true);
            found = m_implementation.soundMap.TryGetValue(soundName, out sound);
            if (!found)
                return channelId;
        }

        FMOD.Channel channel;
        FMOD.RESULT result = m_implementation.system.playSound(sound, default(FMOD.ChannelGroup), true, out channel);
        ErrorCheck(result);
        if (result == FMOD.RESULT.OK)
        {
            FMOD.MODE currMode;
            sound.getMode(out currMode);
            if ((currMode & FMOD.MODE._3D) > 0)
            {
                FMOD.VECTOR pos = VectorToFmod(position);
                FMOD.VECTOR temp = new FMOD.VECTOR();
                ErrorCheck(channel.set3DAttributes(ref pos, ref temp));
            }
            ErrorCheck(channel.setVolume(dbToVolume(volumeDB)));
            ErrorCheck(channel.setPaused(false));
            m_implementation.channelMap[channelId] = channel;
        }
        return channelId;
    }

    public static void SetChannel3dPosition(int channelId, Vector3 position)
    {
        FMOD.Channel channel;
        bool found = m_implementation.channelMap.TryGetValue(channelId, out channel);
        if (!found)
            return;
        
        FMOD.VECTOR pos = VectorToFmod(position);
        FMOD.VECTOR temp = new FMOD.VECTOR();
        ErrorCheck(channel.set3DAttributes(ref pos, ref temp));
    }

    public static void SetChannelVolume(int channelId, float volumeDb)
    {
        FMOD.Channel channel;
        bool found = m_implementation.channelMap.TryGetValue(channelId, out channel);
        if (!found)
            return;

        ErrorCheck(channel.setVolume(dbToVolume(volumeDb)));
    }

    public static void LoadBank(string bankName, FMOD.Studio.LOAD_BANK_FLAGS flags)
    {
        bool found = m_implementation.bankMap.ContainsKey(bankName);
        if (found)
            return;
        
        FMOD.Studio.Bank bank;
        FMOD.RESULT result = m_implementation.studioSystem.loadBankFile(bankName, flags, out bank);
        ErrorCheck(result);
        if (result == FMOD.RESULT.OK)
        {
            m_implementation.bankMap[bankName] = bank;
        }
    }

    public static void LoadEvent(string eventName)
    {
        bool found = m_implementation.eventMap.ContainsKey(eventName);
        if (found)
            return;
        
        FMOD.Studio.EventDescription eventDescription;
        FMOD.RESULT result = m_implementation.studioSystem.getEvent(eventName, out eventDescription);
        ErrorCheck(result);
        if (result == FMOD.RESULT.OK)
        {
            FMOD.Studio.EventInstance eventInstance;
            result = eventDescription.createInstance(out eventInstance);
            ErrorCheck(result);
            if (result == FMOD.RESULT.OK)
            {
                m_implementation.eventMap[eventName] = eventInstance;
            }
        }
    }

    public static void PlayEvent(string eventName)
    {
        FMOD.Studio.EventInstance eventInstance;
        bool found = m_implementation.eventMap.TryGetValue(eventName, out eventInstance);
        if (!found)
        {
            LoadEvent(eventName);
            found = m_implementation.eventMap.TryGetValue(eventName, out eventInstance);
            if (!found)
                return;
        }

        eventInstance.start();
    }

    public static void StopEvent(string eventName, bool immediate)
    {
        FMOD.Studio.EventInstance eventInstance;
        bool found = m_implementation.eventMap.TryGetValue(eventName, out eventInstance);
        if (!found)
        {
            return;
        }

        FMOD.Studio.STOP_MODE mode;
        mode = immediate ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT;
        ErrorCheck(eventInstance.stop(mode));
    }

    public static bool IsEventPlaying(string eventName)
    {
        FMOD.Studio.EventInstance eventInstance;
        bool found = m_implementation.eventMap.TryGetValue(eventName, out eventInstance);
        if (!found)
        {
            return false;
        }

        FMOD.Studio.PLAYBACK_STATE state;
        FMOD.RESULT result = eventInstance.getPlaybackState(out state);
        ErrorCheck(result);
        return result == FMOD.RESULT.OK && state == FMOD.Studio.PLAYBACK_STATE.PLAYING;
    }

    public static void GetEventParameter(string eventName, string parameterName, out float parameter)
    {
        FMOD.Studio.EventInstance eventInstance;
        bool found = m_implementation.eventMap.TryGetValue(eventName, out eventInstance);
        if (!found)
        {
            parameter = float.NaN;
            return;
        }

        FMOD.RESULT result = eventInstance.getParameterByName(parameterName, out parameter);
        ErrorCheck(result);
        if (result != FMOD.RESULT.OK)
        {
            parameter = float.NaN;
        }
    }

    public static void SetEventParameter(string eventName, string parameterName, float value)
    {
        FMOD.Studio.EventInstance eventInstance;
        bool found = m_implementation.eventMap.TryGetValue(eventName, out eventInstance);
        if (!found)
        {
            return;
        }

        ErrorCheck(eventInstance.setParameterByName(parameterName, value));
    }

    public static FMOD.VECTOR VectorToFmod(Vector3 positon)
    {
        FMOD.VECTOR vec = new FMOD.VECTOR();
        vec.x = positon.x;
        vec.y = positon.y;
        vec.z = positon.z;
        return vec;
    }

    public static float dbToVolume(float db)
    {
        return Mathf.Pow(10, 0.05f * db);
    }

    public static float VolumeToDb(float volume)
    {
        return 20.0f * Mathf.Log(volume);
    }
}
