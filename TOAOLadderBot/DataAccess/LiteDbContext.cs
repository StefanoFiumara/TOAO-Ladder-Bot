using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiteDB;

namespace TOAOLadderBot.DataAccess
{
    public class LiteDbContext
    {
        public LiteDatabase Database { get; }
        private readonly List<Action> _commands;

        public LiteDbContext(LiteDatabase database)
        {
            Database = database;
            _commands = new List<Action>();
        }
        
        public void AddCommand(Action command)
        {
            _commands.Add(command);
        }

        public int Save()
        {
            if (_commands.Count == 0)
            {
                return 0;
            }
            
            try
            {
                Database.BeginTrans();
                
                foreach (var command in _commands)
                {
                    command();
                }

                int executedCount = _commands.Count;

                _commands.Clear();
                Database.Commit();
                
                return executedCount;
            }
            catch (LiteException e)
            {
                // TODO: Handle more gracefully?
                Debugger.Break();
                Console.WriteLine(e);
                Database.Rollback();
                throw;
            }
        }

        public int Rollback()
        {
            if (_commands.Count == 0)
            {
                return 0;
            }

            var count = _commands.Count;

            _commands.Clear();

            return count;
        }
    }
}