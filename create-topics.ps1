$topics = @("rawData", "processedData", "dead-letter")

foreach ($topic in $topics) {
    Write-Host "Creating topic: $topic"
    docker exec kafka kafka-topics `
        --create `
        --if-not-exists `
        --topic $topic `
        --bootstrap-server localhost:9092 `
        --partitions 1 `
        --replication-factor 1
}

Write-Host "Topics created."


