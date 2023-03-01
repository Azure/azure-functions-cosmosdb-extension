import { AzureFunction, Context } from "@azure/functions"
import { BSON } from "bson"

const cosmosDBTrigger: AzureFunction = async function (context: Context, bytes: Uint8Array): Promise<void> {
    var docs = BSON.deserialize(bytes).results
    if (!!docs && docs.length > 0) {
        context.log('Document Id: ', docs[0]._id);
    }
}

export default cosmosDBTrigger;
