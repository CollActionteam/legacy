import React from "react";
import { graphql, StaticQuery } from "gatsby";

class Find extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            categories: [],
            projects: [],
            category: null,
            loaded: false
        };
    }

    async componentDidMount() {
        let fetched = 
            await this.getGraphQl(`
                query {
                    categories {
                        id,
                        name
                    },
                    projects {
                        id,
                        name,
                        url
                    }
                }
            `, null);

        console.info(fetched);
        this.setState({ categories: [{ id: null, name: ""}].concat(fetched.data.categories), projects: fetched.data.projects, loaded: true });
    }
    
    async setCategory(e) {
        let categoryId = e.currentTarget.value.toString();
        var fetched = [];
        if (categoryId == "") {
            fetched = 
                await this.getGraphQl(`
                    query {
                        projects {
                            id,
                            name,
                            url
                        }
                    }`, null);
        }
        else {
            fetched = 
                await this.getGraphQl(`
                    query FindProjects($categoryId: [String]) {
                        projects(where: { path: "categoryId", comparison:equal, value: $categoryId}) {
                            id,
                            name,
                            url
                        }
                    }`,
                    JSON.stringify({ "categoryId": categoryId })
                );
        }

        console.info(fetched);
        this.setState({ projects: fetched.data.projects, category: categoryId, loaded: true });
    }

    async getGraphQl(query, variables) {
        let url = this.props.backendUrl + 
                  `graphql?query=${encodeURIComponent(query)}`;
        if (variables !== null) {
            url += `&variables=${encodeURIComponent(variables)}`;
        }
        console.info(url);
        let result = await fetch(url, { method: "GET" });
        console.info(result);
        return result.json();
    }

    async postGraphQl(query, variables) {
        let url = this.props.backendUrl + "graphql";
        let body =
        {
            query: query,
            variables: variables
        };
        console.info(url);
        let result = await fetch(
            url, 
            {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(body)
            });
        console.info(result);
        return result.json();
    }

    render() {
        if (this.state.loaded) {
            return <div>
                <label>Categories</label>
                <select onChange={s => this.setCategory(s)}>
                    {this.state.categories.map(c => <option value={c.id}>{c.name}</option>)}}
                </select>
                {this.state.projects.map(p => <div><a href={p.url}>{p.name}</a></div>)}
            </div>;
        } else {
            return <div></div>;
        }
    }
}

export default () => (
  <StaticQuery query={graphql`
      query {
          site {
              siteMetadata {
                  backendUrl
              }
          }
      }
  `} render={({ site }) => <Find backendUrl={site.siteMetadata.backendUrl} />} />);