import express, { Request, Response } from 'express';
import { SearchAPI } from './SearchAPI';

const app = express();
const searchAPI = new SearchAPI();

app.get('/', async (req: Request, res: Response) => {
    try {
      const status = await searchAPI.getStatus();
      res.send(status);
    } catch (error) {
      res.status(500).send('Error connecting to Elasticsearch.');
    }
  });

app.get('/api/Search/:query', searchAPI.searchProducts);

app.listen(searchAPI.serverPort, () => {
  console.log('SearchAPI is running on port' + searchAPI.serverPort);
});