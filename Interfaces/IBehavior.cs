using RogueSharpDemo.Core;
using RogueSharpDemo.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueSharpDemo.Interfaces
{
    public interface IBehavior
    {
        bool Act(Monster monster, CommandSystem commandSystem);
    }
}
