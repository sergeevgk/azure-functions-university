$location="westeurope"
# e.g. $location="westeurope"

$rgname="azurefunctionuni-sergeevgk-rg"
# e.g. $rgname="myfirstfunction-rg"

az group create `
    --name $rgname `
    --location $location `
    --tags "type=temp"

$stname="azurefuncunisergeevgst"
# e.g. $stname="myfirstfunctionst"

az storage account create `
    --name $stname `
    --resource-group $rgname `
    --location $location `
    --sku Standard_LRS `
    --kind StorageV2 `
    --access-tier Cool


$fappname="azurefuncunisergeevgfapp"
az functionapp create `
    --name $fappname `
    --resource-group $rgname `
    --consumption-plan-location $location `
    --storage-account $stname `
    --runtime dotnet `
    --os-type Windows `
    --functions-version 4