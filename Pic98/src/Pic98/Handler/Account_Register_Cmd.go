package Handler

import (
	//"encoding/json"
	"database/sql"
	"fmt"
	"log"
	"net/http"
	"strings"

	_ "github.com/go-sql-driver/mysql"
	"github.com/satori/go.uuid"
)

func Account_Register_Cmd(w http.ResponseWriter, r *http.Request) {
	var result string
	result = "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd 参数错误！\"}"
	param := strings.Split(r.RequestURI, "/")
	if len(param) >= 4 {
		err := r.ParseForm()
		if err != nil {
			result = "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd ParseForm失败\"}"
		} else {
			cmd := r.FormValue("cmd")
			if cmd != "" {
				db, err := sql.Open("mysql", "pic98:vck123456@tcp(106.14.145.51:4000)/mysql")
				if err != nil {
					log.Fatal(err)
				}
				defer db.Close()
				if cmd == "QueryId" {
					name := r.FormValue("name")

					used, _ := Account_Register_Cmd_CheckId(db, name)
					result = fmt.Sprintf("{\"status\":0,\"msg\":\"Account/Register/Id调用成功！\",\"data\":{\"used\":%d}}", used)
				} else if cmd == "Register" {
					u1, _ := uuid.NewV4()
					result = fmt.Sprintf("{\"status\":0,\"msg\":\"Account/Register/Id调用成功！\",\"data\":{\"register\":%d %s}}", 1, u1)
				}
			} else {
				result = "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd 参数cmd不能为空！\"}"
			}
		}
	} else {
	}
	log.Println(result)
	w.Write([]byte(result))

}

// 获取表数据
func Account_Register_Cmd_CheckId(db *sql.DB, name string) (int, error) {
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

func Account_Register_Cmd_Register(db *sql.DB, name string, pwd string) (int, error) {
	return 1, nil
}
