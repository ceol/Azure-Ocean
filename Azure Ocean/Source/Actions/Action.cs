﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureOcean.Components;
using Debug = System.Diagnostics.Debug;

namespace AzureOcean.Actions
{
    public abstract class GameAction 
    {
        public Actor actor;

        public GameAction (Actor actor)
        {
            this.actor = actor;
        }

        public abstract ActionResult Perform();
    }

    public class Wait : GameAction
    {
        public Wait(Actor actor) : base(actor) { }

        public override ActionResult Perform()
        {
            return ActionResult.SUCCESS;
        }
    }

    public class Walk : GameAction 
    {
        Vector direction;

        public Walk(Actor actor, Vector direction) : base(actor)
        {
            this.direction = direction;
        }

        public override ActionResult Perform()
        {
            Vector position = actor.GetPosition();
            if (position == null)
            {
                return ActionResult.FAILURE;
            }
            
            Vector destination = position + direction;
            Stage stage = actor.game.CurrentStage;
            
            if (!stage.IsTraversable(destination))
            {
                return ActionResult.FAILURE;
            }
            
            actor.SetPosition(destination);
            stage.MoveTo(actor.entity, destination);
            return ActionResult.SUCCESS;
        }
    }

    public struct ActionResult
    {
        public bool succeeded;
        public GameAction alternative;

        public static ActionResult SUCCESS
        {
            get { return new ActionResult() { succeeded = true }; }
        }

        public static ActionResult FAILURE
        {
            get { return new ActionResult() { succeeded = false }; }
        }
    }
}