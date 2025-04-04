using System.Collections.Generic;

public class SoundEmitterVault
{
	private int _nextUniqueKey = 0;
	private List<AudioCueKey> _emittersKey;
	private List<SoundEmitter[]> _emittersList;
    public SoundEmitter[] GetAllEmitters()
    {
        // Kết hợp tất cả các mảng SoundEmitter[] thành một mảng duy nhất
        List<SoundEmitter> allEmitters = new List<SoundEmitter>();

        foreach (var emitters in _emittersList)
        {
            allEmitters.AddRange(emitters); // Thêm tất cả các emitter từ mảng vào list
        }

        return allEmitters.ToArray(); // Chuyển danh sách thành mảng và trả về
    }
    public SoundEmitterVault()
	{
		_emittersKey = new List<AudioCueKey>();
		_emittersList = new List<SoundEmitter[]>();
	}

	public AudioCueKey GetKey(AudioCueSO cue)
	{
		return new AudioCueKey(_nextUniqueKey++, cue);
	}

	public void Add(AudioCueKey key, SoundEmitter[] emitter)
	{
		_emittersKey.Add(key);
		_emittersList.Add(emitter);
	}

    
    public AudioCueKey Add(AudioCueSO cue, SoundEmitter[] emitter)
	{
		AudioCueKey emitterKey = GetKey(cue);

		_emittersKey.Add(emitterKey);
		_emittersList.Add(emitter);

		return emitterKey;
	}

	public bool Get(AudioCueKey key, out SoundEmitter[] emitter)
	{
		int index = _emittersKey.FindIndex(x => x == key);

		if (index < 0)
		{
			emitter = null;
			return false;
		}

		emitter = _emittersList[index];
		return true;
	}

	public bool Remove(AudioCueKey key)
	{
		int index = _emittersKey.FindIndex(x => x == key);
		return RemoveAt(index);
	}

	private bool RemoveAt(int index)
	{
		if (index < 0)
		{
			return false;
		}

		_emittersKey.RemoveAt(index);
		_emittersList.RemoveAt(index);

		return true;
	}
}
