using AutoCodes;
using Foundations;
using Foundations.MVVM;
using Foundations.Tickers;
using System;
using System.Collections.Generic;

namespace World.Characters
{
    public interface ICharacterMgrView : IModelView<CharacterMgr>
    {
        void notify_add_character(Character c);
        void notify_remove_character(Character c);
        void notify_on_tick();
        void notify_select_character(Character c);
        void notify_cancel_select_character(Character c);
        void notify_character_is_working(Character c, bool working);
    }


    public class CharacterMgr : Model<CharacterMgr, ICharacterMgrView>,IMgr
    {
        public List<Character> characters = new List<Character>();      
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        int IMgr.priority => m_mgr_priority;
        readonly int m_mgr_priority;

        public Character select_character;

        //==================================================================================================

        public void tick()
        {
            foreach(var c in characters)
            {
                c.tick();   
            }

            foreach (var view in views)
            {
                view.notify_on_tick();
            }
        }

        public void tick1()
        {

        }

        public CharacterMgr(string name,int priority, params object[] args)
        {
            m_mgr_name = name; 
            m_mgr_priority = priority;

            (this as IMgr).init(args);
        }
        void IMgr.fini()
        {
            Mission.instance.detach_mgr(m_mgr_name);

            var ticker = Ticker.instance;
            ticker.remove_tick(m_mgr_name);
            ticker.remove_tick1(m_mgr_name);
        }

        void IMgr.init(params object[] args)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);
            var ticker = Ticker.instance;
            ticker.add_tick(m_mgr_priority,m_mgr_name,tick);
            ticker.add_tick1(m_mgr_priority,m_mgr_name,tick1);
        }

        public Character AddCharacter(int id)
        {
            var c =  new Character();
            roles.TryGetValue(id.ToString(),out role rc);

            c.Init(rc);
            c.Start();

            characters.Add(c);
            foreach(var view in views)
            {
                view.notify_add_character(c);
            }

            return c;
        }

        public void AddCharacter(Character c)
        {
            characters.Add(c);
            foreach (var view in views)
            {
                view.notify_add_character(c);
            }
        }

        public Character RemoveCharacter(Character c)
        {
            SetCharacterWorking(c, false);
            characters.Remove(c);
            foreach(var view in views)
            {
                view.notify_remove_character(c);
            }
            return c;
        }

        public void SelectCharacter(Character c)
        {
            if (select_character != null)
            {
                CancelSelectCharacter(select_character);
            }

            select_character = c;
            foreach(var view in views)
            {
                view.notify_select_character(c);
            }
        }

        public void CancelSelectCharacter(Character c)
        {
            select_character = null;
            foreach(var view in views)
            {
                view.notify_cancel_select_character(c);
            }
        }

        public void SetCharacterWorking(Character c,bool working)
        {
            if (working)
            {
                c.StartWork();
            }
            else
            {
                c.EndWork();
            }

            foreach (var view in views)
            {
                view.notify_character_is_working(c, working);
            }
        }
    }
}





