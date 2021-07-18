namespace CaPPMS.Shared
{
    public class FakeUser
    {
        private string firstName;
        private string lastName;

        private string email;

        public string getName(){
            return this.firstName+" "+this.lastName;
        }

        public string getEmail(){
            return this.email;
        }

        public FakeUser(string firstName, string lastName, string email){
            this.firstName=firstName;
            this.lastName=lastName;
            this.email=email;
        }

    }
}
