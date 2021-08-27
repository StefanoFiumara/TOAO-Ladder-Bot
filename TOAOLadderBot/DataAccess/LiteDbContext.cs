using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiteDB;

namespace TOAOLadderBot.DataAccess
{
    public class LiteDbContext
    {
        private readonly LiteDatabase _database;
        private readonly List<Action> _commands;

        public LiteDbContext(LiteDatabase database)
        {
            _database = database;
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
                _database.BeginTrans();
                
                foreach (var command in _commands)
                {
                    command();
                }

                int executedCount = _commands.Count;

                _commands.Clear();
                _database.Commit();
                
                return executedCount;
            }
            catch (Exception e)
            {
                Debugger.Break();
                Console.WriteLine(e);
                _database.Rollback();
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