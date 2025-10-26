import fetch from 'node-fetch';

export class ApiClient {
  async get(uri: string) {
    const options = {
      uri: uri,
      headers: {
        'Content-Type': 'application/json',
      },
      json: true, // Automatically parses the JSON string in the response
    };

    const test = await fetch(options.uri, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
    })
      .then((res) => res.json())
      .then((json) => {
        return json;
      });

    return test;
  }
}
