import React, { useState } from "react";
import Layout from "../../components/Layout";
import { Banner } from "../../components/Banner";
import { graphql, StaticQuery } from "gatsby";
import { useQuery } from "react-apollo";
import ProjectsList from "../../components/ProjectsList";
import gql from "graphql-tag";
import { Section } from "../../components/Section";
import { ProjectStatusFilter } from "../../api/types";
import styles from "./find.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Loader from "../../components/Loader";

export default () => (
  <StaticQuery
    query={graphql`
      query FindQuery {
        photos: allFindprojectYaml(
          filter: { name: { eq: "findprojectphotos" } }
        ) {
          edges {
            node {
              findprojectphoto
              name
            }
          }
        }
      }
    `}
    render={staticData => {
      const photos = staticData.photos.edges
        .map(e => e.node)
        .find(n => (n.name = "photos"));

      const [category, setCategory] = useState("");
      const [status, setStatus] = useState(ProjectStatusFilter.Active);
      const { data, loading } = useQuery(GET_CATEGORIES);

      const handleCategoryChange = (e: React.ChangeEvent) => {
        setCategory((e.target as any).value.toString());
      };

      const handleStatusChange = (e: React.ChangeEvent) => {
        setStatus((e.target as any).value);
      };

      return (
        <Layout>
          <Banner photo={photos.findprojectphoto} dots={true}>
            <Section>
              {loading ? (
                <Loader />
              ) : (
                <div className={styles.filter}>
                  <span>Show me</span>

                  <div className={styles.selectWrapper}>
                    <select value={category} onChange={handleCategoryChange}>
                      <option value="">All</option>
                      {data
                        ? data.__type.enumValues.map(v => (
                            <option key={v.name} value={v.name}>
                              {v.name}
                            </option>
                          ))
                        : null}
                    </select>
                    <FontAwesomeIcon icon="angle-down"></FontAwesomeIcon>
                  </div>

                  <span>projects which are</span>

                  <div className={styles.selectWrapper}>
                    <select value={status} onChange={handleStatusChange}>
                      <option value={ProjectStatusFilter.Active}>Open</option>
                      <option value={ProjectStatusFilter.Closed}>Closed</option>
                      <option value={ProjectStatusFilter.ComingSoon}>
                        Coming soon
                      </option>
                    </select>
                    <FontAwesomeIcon icon="angle-down"></FontAwesomeIcon>
                  </div>
                </div>
              )}
            </Section>
          </Banner>
          <Section>
            <ProjectsList category={category} status={status} />
          </Section>
        </Layout>
      );
    }}
  />
);

const GET_CATEGORIES = gql`
  query {
    __type(name:"Category") {
      enumValues {
        name
      }
    }
  }`;