package rabbitmq

import "time"

type CustomTime struct {
	time.Time
}

func (t *CustomTime) UnmarshalJSON(b []byte) (err error) {
	date, err := time.Parse(`"2006-01-02T15:04:05.0000000+00:00"`, string(b))
	if err != nil {
		return err
	}
	t.Time = date
	return
}
