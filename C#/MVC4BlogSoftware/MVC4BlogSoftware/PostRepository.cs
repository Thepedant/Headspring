﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace FirstSteps
{
    public interface IPostRepository
    {
        bool Empty { get; }
        void Add(Post post);
        void Delete(Post post);
        Post[] GetAll(int page, int perPage);
        Post Get(string nickname);
        Post GetMostRecent();
        Post[] GetYear(int year, int page, int perPage);
        Post[] GetAuthor(string author, int page, int perPage);
    }


    //    public class AuthorRepository
//    {
//        public Author[] GetAll()
//        {
//            using (var connection = new MySqlConnection("Server=localhost;Database=blog;Uid=root;Pwd=PASSWORD;"))
//            {
//                connection.Open();
//                var someVar = connection.Query<Author>("select * from authors").ToArray();
//                return someVar;
//            }
//        }
//    }

    public class PostRepository : IPostRepository
    {
        private const string selectClause = "SELECT posts.*, authors.username AS author " +
                                            "FROM posts " +
                                            "INNER JOIN authors ON posts.authorId = authors.id ";

        private const string orderByPublishedDatetimeDesc = "ORDER BY published_datetime DESC ";

        private const string pageClause = "LIMIT @offset, @perPage ";

        public bool Empty { get
        {
            using (var connection = Connection())
            {
                return connection.Query<int>("SELECT COUNT(*) FROM posts").ToArray()[0] == 0;
            }
        } }
        private static readonly StubPostRepository FakeDb;
        static PostRepository()
        {
            FakeDb = new StubPostRepository();
            FakeDb.FillWithFakeData();
        }

        private static SqlConnection Connection()
        {
            var mySqlConnection = new SqlConnection("Server=.\\sqlExpress;Database=blog;Trusted_Connection=true;");
            mySqlConnection.Open();
            return mySqlConnection;
        }

        private static Post[] ConnectionQuery(string queryString = "", object referencedParams = null)
        {
            using (var connection = Connection())
            {
                return connection.Query<Post>(selectClause +
                                                     queryString,
                                                     referencedParams).ToArray();
            }
        }

        public Post[] GetAll(int page, int perPage)
        {
            if(page == 0)
            {
                using (var connection = Connection())
                {
                    return connection.Query<Post>(
                                                  "SELECT posts.*, authors.username AS author " +
                                                  "FROM posts " +
                                                  "INNER JOIN authors ON posts.authorId = authors.id " +
                                                  orderByPublishedDatetimeDesc).ToArray();
                }
            }
            var offset = --page*perPage;
            using (var connection = Connection())
            {
                return connection.Query<Post>("SELECT * " +
                                              "FROM ( " +
                                              "SELECT posts.*, authors.username AS author, ROW_NUMBER() OVER ( " + orderByPublishedDatetimeDesc + ") AS rowNum " +
                                              "FROM posts " +
                                              "INNER JOIN authors ON posts.authorId = authors.id " +
                                              ") AS innerQuery " +
                                              "WHERE innerQuery.rowNum > @offset AND innerQuery.rowNum <= @offset + @perPage ", new {offset, perPage}).ToArray();
            }
        }

        public Post Get(string nickname)
        {

            return ConnectionQuery("WHERE posts.nickname = @nickname;", new {nickname}).FirstOrDefault();
        }

        public Post GetMostRecent()
        {
            return ConnectionQuery(orderByPublishedDatetimeDesc +
                                   "LIMIT 1;").FirstOrDefault();
        }

        public Post[] GetYear(int year, int page, int perPage)
        {
            var offset = page == 0 ? page : --page * perPage;
            return ConnectionQuery("WHERE extract(year from posts.published_datetime) =  @year " +
                                   orderByPublishedDatetimeDesc + pageClause, 
                                   new { year, offset, perPage });
        }

        public Post[] GetAuthor(string author, int page, int perPage)
        {
            var offset = page == 0 ? page : --page * perPage;
            return ConnectionQuery("WHERE authors.username = @author " +
                                   orderByPublishedDatetimeDesc + pageClause, 
                                   new {author, offset, perPage});
        }

        public void Add(Post post)
        {
            FakeDb.Add(post);
        }

        public void Delete(Post post)
        {
            FakeDb.Delete(post);
        }
    }

    public class StubPostRepository : IPostRepository
    {
        //Maintains a collection of Post objects.
        //You can start out initializing the array with about 5 fake posts (new Post(...)...)
        public List<Post> posts;
        public bool Empty 
        { 
            get { return posts.Count == 0; } 
        }

        public StubPostRepository()
        {
            posts = new List<Post>();

        }

        public void FillWithFakeData()
        {
            posts.Add(new Post(1, "First", "first post", new DateTime(34, 4, 23), "John Doe"));
            posts.Add(new Post(2, "Second", "second post", new DateTime(98, 7, 25), "John Doe"));
            posts.Add(new Post(3, "Third", "third post", new DateTime(34, 6, 23), "John Doe"));
            posts.Add(new Post(4, "Fourth", "fourth post", new DateTime(53, 7, 13), "Some Guy"));
            posts.Add(new Post(5, "Fifth", "fifth post", new DateTime(26, 6, 27), "John Doe"));
        }

        public void Add(Post post)
        {
            posts.Add(post);
        }
        public void Delete(Post post)
        {
            posts.Remove(post);
        }

        public Post[] GetAll(int page, int perPage)
        {
            throw new NotImplementedException();
        }

        public Post[] GetAll()
        {
            var collection = new Post[posts.Count()];
            int i = 0;
            foreach (Post post in posts)
            {
                collection[i] = post;
                i++;
            }
            return collection;
        }
        public Post Get(string nickname)
        {
   

//            foreach (Post post in posts)
//            {
//                if (post.Nickname == nickname) return post;
//            }
//            return null;
            //suggested converting to LINQ?
            var returnPost = posts.FirstOrDefault(post => post.Nickname == nickname);
            if (returnPost == null)
            {
                throw new Exception("No post with that nickname.");
            }
            return returnPost;
        }

        public Post GetMostRecent()
        {
            throw new NotImplementedException();
        }

        public Post[] GetYear(int year, int page, int perPage)
        {
            throw new NotImplementedException();
        }

        public Post[] GetYear(int year)
        {
            throw new NotImplementedException();
        }

        public Post[] GetAuthor(string author, int page, int perPage)
        {
            throw new NotImplementedException();
        }
    }
}
