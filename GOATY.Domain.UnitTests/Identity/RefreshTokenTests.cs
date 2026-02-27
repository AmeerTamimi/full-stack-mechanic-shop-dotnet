using GOATY.Domain.Common.Results;
using GOATY.Domain.Identity;
using Newtonsoft.Json.Linq;

namespace GOATY.Domain.UnitTests.Identity
{
    public sealed class RefreshTokenTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var id = Guid.NewGuid();
            var userId = "UserId";
            var token = "Access Token";
            var expiration = DateTimeOffset.Now.AddMinutes(300);

            var actual = RefreshToken.Create(id, userId, token, expiration);

            Assert.True(actual.IsSuccess);
            Assert.Equal(id, actual.Value.Id);
            Assert.Equal(userId, actual.Value.UserId);
            Assert.Equal(token,actual.Value.Token);
            Assert.Equal(expiration, actual.Value.Expiration);
        }

        [Fact]
        public void Create_WithInValidId_ShouldFail()
        {
            var id = Guid.Empty;
            var userId = "UserId";
            var token = "Access Token";
            var expiration = DateTimeOffset.Now.AddMinutes(300);

            var actual = RefreshToken.Create(id, userId, token, expiration);

            var expected = RefreshTokenErrors.IdRequired;

            Assert.False(actual.IsSuccess);
            Assert.Equal(actual.Error , expected);
        }

        [Fact]
        public void Create_WithInValidUserId_ShouldFail()
        {
            var id = Guid.NewGuid();
            string userId = null!;
            var token = "Access Token";
            var expiration = DateTimeOffset.Now.AddMinutes(300);

            var actual = RefreshToken.Create(id, userId, token, expiration);

            var expected = RefreshTokenErrors.UserIdRequired;

            Assert.False(actual.IsSuccess);
            Assert.Equal(actual.Error, expected);
        }

        [Fact]
        public void Create_WithInValidToken_ShouldFail()
        {
            var id = Guid.NewGuid();
            var userId = "UserId";
            var token = "";
            var expiration = DateTimeOffset.Now.AddMinutes(300);

            var actual = RefreshToken.Create(id, userId, token, expiration);

            var expected = RefreshTokenErrors.InvalidToken;

            Assert.False(actual.IsSuccess);
            Assert.Equal(actual.Error, expected);
        }

        [Fact]
        public void Create_WithInValidDate_ShouldFail()
        {
            var id = Guid.NewGuid();
            var userId = "UserId";
            var token = "Access Token";
            var expiration = DateTimeOffset.Now;

            var actual = RefreshToken.Create(id, userId, token, expiration);

            var expected = RefreshTokenErrors.InvalidDate;

            Assert.False(actual.IsSuccess);
            Assert.Equal(actual.Error , expected);
        }
    }
}
