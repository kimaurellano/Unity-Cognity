using System.Linq;
using Assets.Scripts.DataComponent.Database;
using Assets.Scripts.DataComponent.Model;
using UnityEngine;

namespace Assets.Scripts.TestScript {
    public class TestScript : MonoBehaviour {
        private void Start() {
            DatabaseManager databaseManager = new DatabaseManager();
//            databaseManager.UpdateUser("Kim", new User { Username = "Kim Anjelo", FirstRun = false });
//            databaseManager.CreateNewUser(new User{ Username = "Kim Anjelo", FirstRun = false});
//            User user = databaseManager.GetUser("Kim");
//            user.IsLogged = true;
//            user.FirstRun = false;
//            databaseManager.UpdateUser("Kim", user);
//            Debug.Log(databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged)?.Username);
        }
    }
}
