# az-func-c-publish-message

Function C is an Azure function that create messages from CSV and publish them to Azure Service Bus as scheduled messages.

## How to use

1. Create a new Azure Function App
2. Create a new Function
3. Deploy the code from this repository using VS Code or the Azure CLI
4. Create an Azure Service Bus
5. Create a queue (e.g. name it `example_queue`)
6. Add Azure Service Bus connection string to function configuration with the name of `SERVICE_BUS_CONNECTION`
7. Add the name of the queue that you already created (`example_queue`) to function configuration with the name of `QUEUE_NAME`
8. Create Azure Storage Account
9. Create a container called `sensorsdata`
10. Add Azure Storage Account connection string for the atachments container to function configuration with the name of `ccpsattachmentsstorage`

> The function will be triggered if the CSV file added to this path `"path": "sensorsdata/{name}.csv"`
