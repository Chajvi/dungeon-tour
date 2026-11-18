using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonTour.Models;

public sealed class EnemyDefinition
{
    public string Name { get; }
    public string Intro { get; }

    public EnemyDefinition(string name, string intro)
    {
        Name = name;
        Intro = intro;
    }
}