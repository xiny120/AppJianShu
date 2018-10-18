package Handler

import (
	"log"
	"net/http"

	_ "github.com/go-sql-driver/mysql"
)

func Image_Vip(w http.ResponseWriter, r *http.Request) {
	//fmt.Fprintf(w, "%s", "Image now!")
	log.Println(r.RequestURI)
	//param := strings.Split(r.RequestURI, "/")
	//if len(param) >= 3 {

	//log.Println(len(param))
	//log.Println(param[2])
	filePath := "wwwroot/Image/Vip/" + r.RequestURI[11:] //param[2]
	log.Println(filePath)
	if pe, _ := PathExists(filePath); pe == true {
		http.ServeFile(w, r, filePath)
		return
	}
	/*
		// username: root; password: 123456; database: test
		db, err := sql.Open("mysql", "pic98:vck123456@tcp(106.14.145.51:4000)/mysql")
		if err != nil {
			log.Fatal(err)
		}
		defer db.Close()
		// Insert(db)
		// Update(db)
		// Delete(db)
		Get(db)
	*/

	http.ServeFile(w, r, "wwwroot/Image/siwa.jpg")
	//}
}
