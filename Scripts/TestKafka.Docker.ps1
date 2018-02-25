Echo "Docker/Kafka test script"
Echo "Type some words. End them with <ENTER>. To submit, press <CTRL-C>"
Echo "---"
C:\Users\rsenna\Desktop\kafka_2.11-1.0.0\bin\windows\kafka-console-producer.bat --broker-list localhost:9092 --topic hello-topic
Echo "---"
Echo "Now getting items from the topic"
Echo "Press <CTRL-C> to exit"
Echo "---"
C:\Users\rsenna\Desktop\kafka_2.11-1.0.0\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic hello-topic --from-beginning
