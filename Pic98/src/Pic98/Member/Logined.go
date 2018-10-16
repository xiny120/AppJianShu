package Member

import (
	"database/sql"
	"log"

	_ "github.com/go-sql-driver/mysql"
	"github.com/satori/go.uuid"
)

type userinfo struct {
	online_key        string
	user_uuid         string
	un                string
	idol              bool
	writer            bool
	distributor_level int
	distributor_ref0  int
	distributor_ref1  int
}

var (

	//sessions := map[string]userinfo{}
	sessions = make(map[string]userinfo)
)

// NewV1 returns UUID based on current timestamp and MAC address.
func Login(un string, pwd string) (string, error) {
	ui := userinfo{online_key: ""}
	db, err := sql.Open("mysql", "pic98:vck123456@tcp(106.14.145.51:4000)/Pic98")
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()
	//strsql := fmt.Sprintf("SELECT userguid FROM useridentify where userid='%s'", name)
	stmt, _ := db.Prepare(`SELECT a.userguid FROM Pic98.useridentify a, Pic98.userinfo b 
	where a.userid=? and a.userguid = b.userguid and b.password = ?`)
	log.Println(stmt)
	defer stmt.Close()
	rows, err := stmt.Query(un, pwd)
	log.Println(rows)
	log.Println(err)
	if err == nil {
		defer rows.Close()
		if rows.Next() {
			ok, _ := uuid.NewV4()
			ui.online_key = ok.String()
			ui.un = un

			sessions[ui.online_key] = ui
		}
	}
	return ui.online_key, nil
}
