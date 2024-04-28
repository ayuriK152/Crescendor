//
// SmfLite.cs - A minimal toolkit for handling standard MIDI files (SMF) on Unity
//
// Copyright (C) 2013 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace SmfLite
{
    // An alias for internal use.
    using DeltaEventPairList = System.Collections.Generic.List<SmfLite.MidiTrack.DeltaEventPair>;

    //
    // MIDI event
    //
    public struct MidiEvent
    {
        #region Public members

        public byte status;
        public byte data1;
        public byte data2;

        public MidiEvent (byte status, byte data1, byte data2)
        {
            this.status = status;
            this.data1 = data1;
            this.data2 = data2;
        }
        
        public override string ToString ()
        {
            return "[" + status.ToString ("X") + "," + data1 + "," + data2 + "]";
        }

        #endregion
    }

    public class Tempo
    {
        public int deltaTime;
        public int milliSecond;
        public int tempo;

        public Tempo(int deltaTime, int milliSecond)
        {
            this.deltaTime = deltaTime;
            this.milliSecond = milliSecond;
        }
    }

    public class Beat
    {
        public int deltaTime;
        public int numerator;
        public int denominator;
        public int metronomeTick;
        public int demisemiquaverCnt;

        public Beat(int deltaTime, int numerator, int denominator, int metronomeTick, int demisemiquaverCnt)
        {
            this.deltaTime = deltaTime;
            this.numerator = numerator;
            this.denominator = denominator;
            this.metronomeTick = metronomeTick;
            this.demisemiquaverCnt = demisemiquaverCnt;
        }
    }

    //
    // MIDI track
    //
    // Stores only one track (usually a MIDI file contains one or more tracks).
    //
    public class MidiTrack
    {
        #region Internal data structure

        // Data pair storing a delta-time value and an event.
        public struct DeltaEventPair
        {
            public int delta;
            public MidiEvent midiEvent;
            
            public DeltaEventPair (int delta, MidiEvent midiEvent)
            {
                this.delta = delta;
                this.midiEvent = midiEvent;
            }
            
            public override string ToString ()
            {
                return "(" + delta + ":" + midiEvent + ")";
            }
        }

        #endregion

        #region Public members

        public MidiTrack ()
        {
            sequence = new List<DeltaEventPair> ();
        }

        // Returns an enumerator which enumerates the all delta-event pairs.
        public List<DeltaEventPair>.Enumerator GetEnumerator ()
        {
            return sequence.GetEnumerator ();
        }
        
        public DeltaEventPair GetAtIndex (int index)
        {
            return sequence [index];
        }
        
        public override string ToString ()
        {
            var s = "";
            foreach (var pair in sequence)
                s += pair;
            return s;
        }

        #endregion

        #region Private and internal members

        public List<DeltaEventPair> sequence;

        public void AddEvent (int delta, MidiEvent midiEvent)
        {
            sequence.Add (new DeltaEventPair (delta, midiEvent));
        }

        #endregion
    }
    
    //
    // MIDI file container
    //
    public struct MidiFileContainer
    {
        #region Public members

        // Division number == PPQN for this song.
        public int division;

        // Track list contained in this file.
        public List<MidiTrack> tracks;

        public List<Tempo> tempoMap;

        public List<Beat> beatMap;
        
        public MidiFileContainer (int division, List<MidiTrack> tracks, List<Tempo> tempoMap, List<Beat> beatMap)
        {
            this.division = division;
            this.tracks = tracks;
            this.tempoMap = tempoMap;
            this.beatMap = beatMap;
        }
        
        public override string ToString ()
        {
            var temp = division.ToString () + ",";
            foreach (var track in tracks) {
                temp += track;
            }
            return temp;
        }

        #endregion
    }

    //
    // Sequencer for MIDI tracks
    //
    // Works like an enumerator for MIDI events.
    // Note that not only Advance() but also Start() can return MIDI events.
    //
    public class MidiTrackSequencer
    {
        #region Public members

        public bool Playing {
            get { return playing; }
        }

        // Constructor
        //   "ppqn" stands for Pulse Per Quater Note,
        //   which is usually provided with a MIDI header.
        public MidiTrackSequencer (MidiTrack track, int ppqn, float bpm)
        {
            pulsePerSecond = bpm / 60.0f * ppqn;
            enumerator = track.GetEnumerator ();
        }

        // Start the sequence.
        // Returns a list of events at the beginning of the track.
        public List<MidiEvent> Start (float startTime = 0.0f)
        {
            if (enumerator.MoveNext ()) {
                pulseToNext = enumerator.Current.delta;
                playing = true;
                return Advance (startTime);
            } else {
                playing = false;
                return null;
            }
        }

        // Advance the song position.
        // Returns a list of events between the current position and the next one.
        public List<MidiEvent> Advance (float deltaTime)
        {
            if (!playing) {
                return null;
            }
            
            pulseCounter += pulsePerSecond * deltaTime;
            
            if (pulseCounter < pulseToNext) {
                return null;
            }
            
            var messages = new List<MidiEvent> ();
            
            while (pulseCounter >= pulseToNext) {
                var pair = enumerator.Current;
                messages.Add (pair.midiEvent);
                if (!enumerator.MoveNext ()) {
                    playing = false;
                    break;
                }
                
                pulseCounter -= pulseToNext;
                pulseToNext = enumerator.Current.delta;
            }
            
            return messages;
        }

        #endregion

        #region Private members

        DeltaEventPairList.Enumerator enumerator;
        bool playing;
        float pulsePerSecond;
        float pulseToNext;
        float pulseCounter;

        #endregion
    }

    //
    // MIDI file loader
    //
    // Loads an SMF and returns a file container object.
    //
    public static class MidiFileLoader
    {
        #region Public members

        public static MidiFileContainer Load (byte[] data)
        {
            var tracks = new List<MidiTrack> ();
            var tempoMap = new List<Tempo>();
            var beatMap = new List<Beat>();
            var reader = new MidiDataStreamReader (data);
            
            // Chunk type.
            if (new string (reader.ReadChars (4)) != "MThd") {
                throw new System.FormatException ("Can't find header chunk.");
            }
            
            // Chunk length.
            if (reader.ReadBEInt32 () != 6) {
                throw new System.FormatException ("Length of header chunk must be 6.");
            }
            
            // Format (unused).
            reader.Advance (2);
            
            // Number of tracks.
            var trackCount = reader.ReadBEInt16 ();
            
            // Delta-time divisions.
            var division = reader.ReadBEInt16 ();
            if ((division & 0x8000) != 0) {
                throw new System.FormatException ("SMPTE time code is not supported.");
            }
            
            // Read the tracks.
            for (var trackIndex = 0; trackIndex < trackCount; trackIndex++) {
                KeyValuePair<MidiTrack, KeyValuePair<List<Tempo>, List<Beat>>> result = ReadTrack(reader);
                tracks.Add(result.Key);
                if (result.Value.Key.Count > 0)
                    tempoMap = result.Value.Key;
                if (result.Value.Value.Count > 0)
                    beatMap = result.Value.Value;
            }


            
            return new MidiFileContainer (division, tracks, tempoMap, beatMap);
        }

        #endregion

        #region Private members
        
        static KeyValuePair<MidiTrack, KeyValuePair<List<Tempo>, List<Beat>>> ReadTrack (MidiDataStreamReader reader)
        {
            var track = new MidiTrack ();
            var tempoMap = new List<Tempo>();
            var beatMap = new List<Beat>();

            // Chunk type.
            string trackName = new string(reader.ReadChars(4));
            if (trackName != "MTrk") {
                throw new System.FormatException ("Can't find track chunk.");
            }
            
            // Chunk length.
            var chunkEnd = reader.ReadBEInt32 ();
            chunkEnd += reader.Offset;
            
            // Read delta-time and event pairs.
            byte ev = 0;
            while (reader.Offset < chunkEnd) {
                // Delta time.
                var delta = reader.ReadMultiByteValue ();
                
                // Event type.
                if ((reader.PeekByte () & 0x80) != 0) {
                    ev = reader.ReadByte ();
                    if (ev == 0x81 || ev == 0x91)
                        ev -= 1;
                }
                
                if (ev == 0xff) {
                    // 0xff: Meta event
                    // 템포를 읽기위한 임의 수정 부분
                    ev = reader.ReadByte();
                    if (ev == 0x58) {
                        // 0x51: Read Tempo
                        List<int> datas = new List<int>();
                        byte length = reader.ReadByte();
                        for (int i = 0; i < length; i++) {
                            datas.Add(reader.ReadByte());
                        }
                        beatMap.Add(new Beat(delta, datas[0], (int)Math.Pow(2, datas[1]) , datas[2], datas[3]));
                    } else if (ev == 0x51) {
                        // 0x51: Read Tempo
                        byte length = reader.ReadByte();
                        int value = 0;
                        for (int i = 0; i < length; i++) {
                            value <<= 8;
                            int tempByte = reader.ReadByte();
                            value += tempByte & 0xff;
                        }
                        tempoMap.Add(new Tempo(delta, value));
                    } else {
                        reader.Advance(reader.ReadMultiByteValue());
                    }
                } else if (ev == 0xf0) {
                    // 0xf0: SysEx (unused).
                    while (reader.ReadByte() != 0xf7) {
                    }
                } else {
                    // MIDI event
                    byte data1 = reader.ReadByte ();
                    byte data2 = ((ev & 0xe0) == 0xc0) ? (byte)0 : reader.ReadByte ();
                    track.AddEvent (delta, new MidiEvent (ev, data1, data2));
                }
            }

            return new KeyValuePair<MidiTrack, KeyValuePair<List<Tempo>, List<Beat>>>(track, new KeyValuePair<List<Tempo>, List<Beat>>(tempoMap, beatMap));
        }

        #endregion
    }

    //
    // Binary data stream reader (for internal use)
    //
    class MidiDataStreamReader
    {
        byte[] data;
        int offset;

        public int Offset {
            get { return offset; }
        }

        public MidiDataStreamReader (byte[] data)
        {
            this.data = data;
        }

        public void Advance (int length)
        {
            offset += length;
        }

        public byte PeekByte ()
        {
            return data [offset];
        }

        public byte ReadByte ()
        {
            return data [offset++];
        }

        public char[] ReadChars (int length)
        {
            var temp = new char[length];
            for (var i = 0; i < length; i++) {
                temp [i] = (char)ReadByte ();
            }
            return temp;
        }

        public int ReadBEInt32 ()
        {
            int b1 = ReadByte ();
            int b2 = ReadByte ();
            int b3 = ReadByte ();
            int b4 = ReadByte ();
            return b4 + (b3 << 8) + (b2 << 16) + (b1 << 24);
        }
        
        public int ReadBEInt16 ()
        {
            int b1 = ReadByte ();
            int b2 = ReadByte ();
            return b2 + (b1 << 8);
        }

        public int ReadMultiByteValue ()
        {
            int value = 0;
            while (true) {
                int b = ReadByte ();
                value += b & 0x7f;
                if (b < 0x80)
                    break;
                value <<= 7;
            }
            return value;
        }
    }
}
