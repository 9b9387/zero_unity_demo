package game

import (
	"encoding/json"
	"log"

	"github.com/9b9387/zero"
)

// HandleMessage 处理网络请求
func HandleMessage(s *zero.Session, msg *zero.Message) {
	msgID := msg.GetID()

	switch msgID {
	case RequestJoin:
		name := s.GetConn().GetName()
		player := CreatePlayer(name, s)

		m := make(map[string]interface{})
		m["self"] = player
		m["list"] = world.GetPlayerList()
		data, _ := json.Marshal(m)

		response := zero.NewMessage(ResponseJoin, data)
		s.GetConn().SendMessage(response)

		for _, p := range world.GetPlayerList() {
			response := zero.NewMessage(BroadcastJoin, player.ToJSON())
			p.Session.GetConn().SendMessage(response)
		}

		world.AddPlayer(player)

		s.BindUserID(player.PlayerID)
		break

	case RequestMove:
		var f interface{}
		err := json.Unmarshal(msg.GetData(), &f)
		if err != nil {
			return
		}
		m := f.(map[string]interface{})
		x := m["x"]
		y := m["y"]
		playerID := m["playerID"].(string)

		player := world.GetPlayer(playerID)
		player.X = int(x.(float64))
		player.Y = int(y.(float64))

		players := world.GetPlayerList()

		for _, p := range players {
			message := zero.NewMessage(BroadcastMove, player.ToJSON())
			p.Session.GetConn().SendMessage(message)
		}
		break
	}
}

// HandleDisconnect 处理网络断线
func HandleDisconnect(s *zero.Session, err error) {
	log.Println(s.GetConn().GetName() + " lost.")
	uid := s.GetUserID()
	lostPlayer := world.GetPlayer(uid)
	if lostPlayer == nil {
		return
	}

	world.RemovePlayer(uid)
	for _, p := range world.GetPlayerList() {
		message := zero.NewMessage(BroadcastLeave, lostPlayer.ToJSON())
		p.Session.GetConn().SendMessage(message)
	}
}

// HandleConnect 处理网络连接
func HandleConnect(s *zero.Session) {
	log.Println(s.GetConn().GetName() + " connected.")
}
