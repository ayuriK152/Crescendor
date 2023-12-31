using SmfLite;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class MidiTest : MonoBehaviour
{
    public TextAsset sourceFile;
    public MidiFileContainer song;
    public GameObject noteObj;
    public Transform noteInstantiatePoint;

    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;

    public float noteScale = 1.0f;

    List<GameObject> instatiateNotes = new List<GameObject>();
    List<Notes> notes = new List<Notes>();
    Dictionary<int, int> tempNoteData = new Dictionary<int, int>();

    void Awake()
    {
        sourceFile = Resources.Load<TextAsset>("Converts/b9IsAwesome");
    }

    IEnumerator Start()
    {
        song = MidiFileLoader.Load(sourceFile.bytes);
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < song.tracks.Count; i++)
        {
            MidiTrack track = song.tracks[i];
            int startTime = -1;
            int deltaTime = 0;
            for (int j = 0; j < track.sequence.Count; j++)
            {
                deltaTime = track.sequence[j].delta != 0 ? track.sequence[j].delta : deltaTime;
                startTime += startTime == -1 ? 1 : track.sequence[j].delta;
                if (track.sequence[j].midiEvent.status == 144)
                {
                    tempNoteData.Add(track.sequence[j].midiEvent.data1, startTime);
                }
                else if (track.sequence[j].midiEvent.status == 128)
                {
                    notes.Add(new Notes(track.sequence[j].midiEvent.data1, tempNoteData[track.sequence[j].midiEvent.data1], startTime));
                    tempNoteData.Remove(track.sequence[j].midiEvent.data1);
                }
            }
        }
        for (int i = 0; i < notes.Count; i++)
        {
            instatiateNotes.Add(Instantiate(noteObj, noteInstantiatePoint));
            instatiateNotes[i].transform.localScale = new Vector3(0.5f, 0.5f, notes[i].deltaTime / 480.0f * noteScale);
            instatiateNotes[i].transform.localPosition = new Vector3(notes[i].keyNum, 0, (notes[i].startTime / 480.0f + notes[i].deltaTime / 960.0f) * noteScale);
        }
    }
}
