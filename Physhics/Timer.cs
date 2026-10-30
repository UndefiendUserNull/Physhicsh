namespace Physhicsh;

public class Timer(float interval, Action? OnEnd, bool loop = false)
{
    private readonly float _interval = interval;
    private float _time = interval;
    public bool IsRunning { get; private set; }
    public bool IsFinished { get; private set; }
    private readonly Action? _onEnd = OnEnd;
    private readonly bool _loop = loop;

    public void Start()
    {
        _time = _interval;
        IsRunning = true;
        IsFinished = false;
    }

    public void Update(float dt)
    {
        if (!IsRunning) return;

        _time -= dt;
        if (_time > 0f) return;

        if (_loop)
        {
            _time = _interval;
        }
        else
        {
            _time = 0f;
            IsFinished = true;
            IsRunning = false;
        }

        _onEnd?.Invoke();
    }

    public void Pause() => IsRunning = false;

    public void Resume()
    {
        if (IsFinished) return;
        IsRunning = true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Timer t)
            return t._interval == _interval;

        return base.Equals(obj);
    }
}