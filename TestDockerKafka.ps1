Echo "Docker/Kafka test script"
Echo "Type some words. End them with <ENTER>. To submit, press <CTRL-C>"
Echo "---"
docker exec -ti kafkadiff_kafka_1 /opt/kafka/bin/kafka-console-producer.sh --broker-list localhost:9092 --topic hello-topic
Echo "---"
Echo "Now getting items from the topic"
Echo "Press <CTRL-C> to exit"
Echo "---"
docker exec -ti kafkadiff_kafka_1 /opt/kafka/bin/kafka-console-consumer.sh --bootstrap-server localhost:9092 --topic hello-topic --from-beginning
