using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ScheduledTask
{
    public string name;
    public float interval;
    public float timer;
    public Action action;

    public ScheduledTask(float _interval, float _timer, Action _action, string _name = "")
    {
        this.name = _name;
        this.interval = _interval;
        this.timer = _timer;
        this.action = _action;
    }
}
