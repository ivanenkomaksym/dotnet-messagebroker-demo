import { readFileSync } from 'fs';
import { Client } from '@elastic/elasticsearch';
import { Product } from './models/product';

export class SearchAPI {
    private config: any;
    private elasticsearchClient: Client;

    constructor() {
        this.config = JSON.parse(readFileSync('config.json', 'utf8'));
        this.elasticsearchClient = new Client({ node: this.config.ElasticSearch.ClientURL });
        console.log('Connecting to elastic via: ' + this.config.ElasticSearch.ClientURL);
    }

    // Get the catalog API URL.
    get gatewayAddress(): string {
        return this.config.ApiSettings.GatewayAddress;
    }

    // Get the Elasticsearch client URL.
    get elasticSearchClientUrl(): string {
        return this.config.ElasticSearch.ClientURL;
    }

    // Get the server port.
    get serverPort(): number {
        return this.config.ServerSettings.Port;
    }

    public async getStatus() {
        try {
          const info = await this.elasticsearchClient.info();
          return `Elasticsearch cluster is up and running.\nCluster name: ${info.cluster_name}`;
        } catch (error) {
          console.error(error);
          throw error;
        }
      }

    // Search for products in the Elasticsearch index.
    async searchProducts(query: string): Promise<Product[]> {
        console.log('SearchAPI.searchProducts ${query}');
        const response = await this.elasticsearchClient.search({
            index: 'products',
            body: {
                query: {
                    match: {
                        name: query,
                    },
                },
            },
        });

        return response.hits.hits.map((hit) => hit._source as Product);
    }
}