use chrono::{DateTime, Utc};
use serde::Serializer;

// Custom serializer for DateTime<Utc>
pub fn serialize_datetime<S>(date_time: &DateTime<Utc>, serializer: S) -> Result<S::Ok, S::Error>
where
    S: Serializer,
{
    let formatted = date_time.format("%Y-%m-%dT%H:%M:%S%.6f%:z").to_string();
    serializer.serialize_str(&formatted)
}
