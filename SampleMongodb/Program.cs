using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace SampleMongodb
{
	class Program
	{
		static async Task Main(string[] args)
		{
			BsonClassMap.RegisterClassMap<User>(cm =>
			{
				cm.AutoMap();
				cm.MapIdMember(m => m.UserId);
				cm.MapField("_photos").SetElementName("Photos");
			});
			BsonClassMap.RegisterClassMap<Photo>(cm =>
			{
				cm.AutoMap();
				cm.MapIdMember(m => m.PhotoId);
			});

			var client = new MongoClient("mongodb://localhost:27017");
			var database = client.GetDatabase("awesomedb");
			var collection = database.GetCollection<User>("User");
			var userA = new User
			{
				UserId = Guid.NewGuid(),
				IdentityId = Guid.NewGuid(),
				City = "taolao",
				Country = "bidao",
				UserName = "taolaobidao",

			};
			userA.AddPhoto(new Photo
			{
				PhotoId = Guid.NewGuid(),
				Description = "abc",
				ExternalId = "dsadsdsa",
			});
			await collection.InsertOneAsync(userA);

			var list = await collection.Find(FilterDefinition<User>.Empty)
				.ToListAsync();

			//foreach (var person in list)
			//{
			//	Console.WriteLine($"{person.Id}: {person.Name}");
			//}
		}
	}

	public class User
	{
		public Guid UserId { get; set; }
		public Guid IdentityId { get; set; }

		public string UserName { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }


		public DateTime DateOfBirth { get; set; }
		public string KnownAs { get; set; }
		public DateTime LastActive { get; set; }
		public string Introduction { get; set; }
		public string LookingFor { get; set; }
		public string Interests { get; set; }
		public string City { get; set; }
		public string Country { get; set; }

		private HashSet<Photo> _photos = new HashSet<Photo>();
		public IEnumerable<Photo> Photos => _photos.ToList();


		public Photo GetPhoto(Guid photoId)
		{
			return _photos.FirstOrDefault(p => p.PhotoId == photoId);
		}

		public string GetMainPhotoUrl()
		{
			return _photos.FirstOrDefault(p => p.IsMain)?.Url ?? "https://randomuser.me/api/portraits/lego/1.jpg";
		}

		public void AddPhoto(Photo photo)
		{
			if (!Photos.Any())
			{
				photo.IsMain = true;
			}

			_photos.Add(photo);
		}

		
	}
	public class Photo
	{
		public Guid PhotoId { get; set; }
		public string Url { get; set; }
		public string Description { get; set; }
		public bool IsMain { get; set; }
		public Guid UserId { get; set; }
		public User User { get; set; }
		public string ExternalId { get; set; }
	}
}
