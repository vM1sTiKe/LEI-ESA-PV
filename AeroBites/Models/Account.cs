namespace AeroBites.Models
{
    public class Account
    {
        //https://console.cloud.google.com/apis/dashboard?inv=1&invt=AbmrFA&project=esa-pv
        public int Id { get; set; }

        public int GoogleId { get; set; }

        public bool IsAdmin { get; set; }



        //vai ter a chave primaria e um google id

        //https://developers.google.com/identity/gsi/web/guides/verify-google-id-token

        //https://developers.google.com/identity/gsi/web/reference/html-reference

//https://www.youtube.com/watch?v=uCpOLZiSb3s
    }
}
