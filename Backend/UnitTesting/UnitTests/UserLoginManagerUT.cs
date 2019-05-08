using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ServiceLayer.Services.ExceptionService;
using ManagerLayer.KFC_SSO_Utility;
using ServiceLayer.KFC_API_Services;
using UnitTesting;


namespace Testing.UnitTests
{
    /// <summary>
    /// Summary description for LoginManagerUT
    /// </summary>
    [TestClass]
    public class UserLoginManagerUT
    {
        readonly TestingUtils ut;
        public KFC_SSO_Manager _ssoLoginManager;

        public UserLoginManagerUT()
        {
            ut = new TestingUtils();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidEmailException))]
        public async void Login_NewUser_InvalidUserName_Failure_ExceptionThrown()
        {
            var invalid_username = Guid.NewGuid() + ".com";
            var valid_ssoID = Guid.NewGuid();
            var timestamp = 8283752242;

            MockLoginPayload mock_payload = new MockLoginPayload
            {
                email = invalid_username,
                ssoUserId = valid_ssoID,
                timestamp = timestamp
            };

            var signature = mock_payload.Signature();

            using (var _db = ut.CreateDataBaseContext())
            {
                _ssoLoginManager = new KFC_SSO_Manager(_db);
                var result = await _ssoLoginManager.LoginFromSSO(invalid_username, valid_ssoID, timestamp, signature);
            }

            //Assert - catch exception
        }

        [TestMethod]
        public void Login_NewUser_ValidUserName_Success()
        {
            using (var _db = ut.CreateDataBaseContext())
            {
                _ssoLoginManager = new KFC_SSO_Manager(_db);
                var user = ut.CreateSSOUserInDb();
                var timestamp = 8283752242;
                MockLoginPayload mock_payload = new MockLoginPayload
                {
                    email = user.Username,
                    ssoUserId = user.Id,
                    timestamp = timestamp
                };

                var response = _ssoLoginManager.LoginFromSSO(mock_payload.email, mock_payload.ssoUserId, timestamp, mock_payload.Signature());
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public void Login_ExistingUser_Success()
        {
            var existing_user = ut.CreateSSOUserInDb();
            var existing_username = existing_user.Username;
            var existing_ssoID = existing_user.Id;
            var timestamp = 12312312;
            var _ssoAuth = new SignatureService();
            MockLoginPayload mock_payload = new MockLoginPayload
            {
                email = existing_username,
                ssoUserId = existing_ssoID,
                timestamp = timestamp
            };
            var signature = mock_payload.Signature();

            using (var _db = ut.CreateDataBaseContext())
            {
                _ssoLoginManager = new KFC_SSO_Manager(_db);
                var response = _ssoLoginManager.LoginFromSSO(existing_username, existing_ssoID, timestamp, signature);
                Assert.IsNotNull(response);
            }
        }
    }
}
