﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Democraticdj.Model;
using MongoDB.Driver;

namespace Democraticdj.Services
{
  public class DbConnector
  {
    private string _databaseName;
    protected string DatabaseName
    {
      get { return _databaseName ?? (_databaseName = MongoUrl.Create(ConnectionString).DatabaseName); }
    }

    private string _connectionString;
    protected string ConnectionString
    {
      get { return _connectionString ?? (_connectionString = ConfigurationManager.AppSettings["MONGOLAB_URI"]); }
    }

    MongoClient _client = new MongoClient();
    MongoClient Client
    {
      get
      {
        MongoUrl mongoUrl = new MongoUrl(ConnectionString);
        return _client ?? (_client = new MongoClient(mongoUrl));
      }
    }

    private IMongoDatabase _database;
    public IMongoDatabase Database
    {
      get { return _database ?? (_database= Client.GetDatabase(DatabaseName)); }
    }

    public Session GetSession(string sessionId)
    {
      return Sessions.Find(storedSession => storedSession.SessionId == sessionId).FirstOrDefault();
    }

    public void SaveSession(Session session)
    {
      var existingSession = Sessions.Find(storedSession => storedSession.SessionId == session.SessionId).FirstOrDefault();
      if (existingSession == null)
      {
        Sessions.InsertOne(session);
      }
      else
      {
        Sessions.ReplaceOne(storedSession => storedSession.SessionId == session.SessionId, session);
      }
    }

    protected IMongoCollection<Session> Sessions
    {
      get { return Database.GetCollection<Session>("sessions"); }
    }

    public User GetUser(string userId)
    {
      return Users.Find(user => user.UserId ==  userId).FirstOrDefault();
    }

    public void SaveUser(User user)
    {
      var existingSession = Users.Find(storedUser => storedUser.UserId== user.UserId).FirstOrDefault();
      if (existingSession == null)
      {
        Users.InsertOne(user);
      }
      else
      {
        Users.ReplaceOne(storedUser => storedUser.UserId== user.UserId, user);
      }
    }

    protected IMongoCollection<User> Users
    {
      get { return Database.GetCollection<User>("users"); }
    }
  }
}