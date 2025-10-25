import { injectable, inject } from 'inversify';
import { GoogleCloudServices } from './google-cloud.services';
import { Logger } from '../../logger';

const axios = require('axios').default;

@injectable()
export class GoogleCloudClient {
  constructor(
    @inject(GoogleCloudServices)
    private googleCloudServices: GoogleCloudServices,
    @inject(Logger) private logger: Logger
  ) {}

  async readImage(url: string) {
    var retVal: string[] | null = null;
    // Get image as buffer
    const bytes = await axios
      .get(url, { responseType: 'arraybuffer' })
      .then((response: any) => {
        return Buffer.from(response.data, 'binary');
      });

    var image = { content: bytes };

    var imageContext = { languageHints: ['en', 'en-GB', 'nl', 'nl-BE'] };

    const request = {
      image: image,
      imageContext: imageContext,
    };

    retVal = await this.googleCloudServices.textClient
      .documentTextDetection(request)
      .then((response: any) => {
        return response[0].textAnnotations[0].description.split('\n');
      })
      .catch((error: any) => {
        this.logger.log(`Error in ${GoogleCloudClient.name}\n${error}`);
      });

    return retVal;
  }
}
