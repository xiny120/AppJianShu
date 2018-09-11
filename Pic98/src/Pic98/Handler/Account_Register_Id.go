package Handler

import (
	//"encoding/json"
	"database/sql"
	"fmt"
	"log"
	"net/http"
	"strings"

	_ "github.com/go-sql-driver/mysql"
)

func Id(w http.ResponseWriter, r *http.Request) {
	var result string
	result = "{'status':1,'msg':'Account/Register/Id参数错误！'}"
	param := strings.Split(r.RequestURI, "/")
	if len(param) >= 4 {
		err := r.ParseForm()
		if err != nil {
			fmt.Println("解析表单数据失败!")
		} else {
			fmt.Println(r.Form)
			fmt.Println(r.PostForm)
			name := r.FormValue("name")
			log.Println(name)

			db, err := sql.Open("mysql", "pic98:vck123456@tcp(106.14.145.51:4000)/mysql")
			if err != nil {
				log.Fatal(err)
			}
			defer db.Close()
			used, err := CheckId(db, name)

			//result = "{'status':0,'msg':'Account/Register/Id调用成功！','data':{'used':'" + used + "'}}"
			result = fmt.Sprintf("{'status':0,'msg':'Account/Register/Id调用成功！','data':{'used':%d}}", used)
		}
	} else {
	}
	log.Println(result)
	w.Write([]byte(result))

}

// 获取表数据
func CheckId(db *sql.DB, name string) (int, error) {
	strsql := fmt.Sprintf("SELECT userguid FROM Pic98.useridentify where userid='%s'", name)
	log.Println(strsql)
	rows, err := db.Query(strsql)
	log.Println(rows)
	log.Println(err)
	if err == nil {
		defer rows.Close()
		if !rows.Next() {
			return 0, nil
		}
	}
	return 1, err
}
