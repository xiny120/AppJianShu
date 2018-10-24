package Handler

import (
	"database/sql"
	"fmt"
	"log"
	"net/http"
	"strings"

	_ "github.com/go-sql-driver/mysql"
)

func Image(w http.ResponseWriter, r *http.Request) {
	log.Println(r.RequestURI)
	param := strings.Split(r.RequestURI, "/")
	if len(param) >= 3 {
		log.Println(len(param))
		log.Println(param[2])
		filePath := "wwwroot/Image/" + param[2]
		log.Println(filePath)
		if pe, _ := PathExists(filePath); pe == true {
			http.ServeFile(w, r, filePath)
			return
		}
		http.ServeFile(w, r, "wwwroot/Image/siwa.jpg")
	}
}

func Image_Banner(w http.ResponseWriter, r *http.Request) {
	param := strings.Split(r.RequestURI, "/")
	if len(param) >= 3 {
		filePath := "wwwroot/Image/Banner"
		for _, val := range param[3:] {
			if val == ".." {
				continue
			}
			filePath = filePath + "/" + val

		}
		log.Println(filePath)
		if pe, _ := PathExists(filePath); pe == true {
			http.ServeFile(w, r, filePath)
			return
		}
		http.ServeFile(w, r, "wwwroot/Image/siwa.jpg")
	}
}

func Image_Vip(w http.ResponseWriter, r *http.Request) {
	param := strings.Split(r.RequestURI, "/")
	if len(param) >= 3 {
		filePath := "wwwroot/Image/Vip"
		for _, val := range param[3:] {
			if val == ".." {
				continue
			}
			filePath = filePath + "/" + val

		}
		log.Println(filePath)
		if pe, _ := FileExists(filePath); pe == true {
			http.ServeFile(w, r, filePath)
			return
		}
		http.ServeFile(w, r, "wwwroot/Image/siwa.jpg")
	}
}

// 获取表数据
func Get1(db *sql.DB) {
	rows, err := db.Query("select * from user;")
	if err != nil {
		log.Fatal(err)
	}
	defer rows.Close()
	cloumns, err := rows.Columns()
	if err != nil {
		log.Fatal(err)
	}
	// for rows.Next() {
	//  err := rows.Scan(&cloumns[0], &cloumns[1], &cloumns[2])
	//  if err != nil {
	//      log.Fatal(err)
	//  }
	//  fmt.Println(cloumns[0], cloumns[1], cloumns[2])
	// }
	values := make([]sql.RawBytes, len(cloumns))
	scanArgs := make([]interface{}, len(values))
	for i := range values {
		scanArgs[i] = &values[i]
	}
	for rows.Next() {
		err = rows.Scan(scanArgs...)
		if err != nil {
			log.Fatal(err)
		}
		var value string
		for i, col := range values {
			if col == nil {
				value = "NULL"
			} else {
				value = string(col)
			}
			fmt.Println(cloumns[i], ": ", value)
		}
		fmt.Println("------------------")
	}
	if err = rows.Err(); err != nil {
		log.Fatal(err)
	}
}
