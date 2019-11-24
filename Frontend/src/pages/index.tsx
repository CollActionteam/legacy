import React from "react";
import Layout from "../components/Layout";
import { graphql, StaticQuery } from "gatsby";
import { CallToAction } from "../components/CallToAction";
import { Hidden } from "@material-ui/core";

import styles from "./index.module.scss";
import { CrowdactingSteps } from "../components/CrowdactingSteps";
import { StartProjectSteps } from "../components/StartProjectSteps";
import { Button } from "../components/Button";
import { Facebook, Twitter, LinkedIn, Email } from "../components/Share";
import { Section } from "../components/Section";
import Carousel from "../components/Carousel";
import ProjectsList from "../components/ProjectsList";

export default () => (
  <StaticQuery
    query={graphql`
      query HomeQuery {
        site {
          siteMetadata {
            title
          }
        }
        content: allMarkdownRemark(
          filter: { frontmatter: { type: { eq: "home" } } }
        ) {
          edges {
            node {
              html
              frontmatter {
                title
                name
              }
            }
          }
        }
      }
    `}
    render={staticData => {
      const sections = staticData.content.edges.map(e => e.node);
      const intro = sections.find(s => s.frontmatter.name === "intro");

      return (
        <Layout>
          <Carousel />
          <Section className={styles.introduction}>
            <h2>{intro.frontmatter.title}</h2>
            <p dangerouslySetInnerHTML={{ __html: intro.html }}></p>
          </Section>
          <Section color="grey">
            <CrowdactingSteps />
          </Section>
          <Hidden smDown>
            <Section className={styles.findProject}>
              <h1>Join a project</h1>
              <ProjectsList />
              <Button to="/projects/find">Find more projects</Button>
            </Section>
            <Section color="grey" className={styles.startproject}>
              <h1>Starting a project</h1>
              <h2>(is super easy)</h2>
              <StartProjectSteps />
              <Button to="/projects/start">Start a project</Button>
            </Section>
          </Hidden>
          <Hidden mdUp>
            <Section>
              <CallToAction title="" />
            </Section>
          </Hidden>
          <Section color="grey">
            <div className={styles.spread}>
              <div className={styles.spreadBlock}>
                <h2>Spread it further!</h2>
                <ul>
                  <li>
                    <Facebook url="https://www.collaction.org" />
                  </li>
                  <li>
                    <Twitter url="https://www.collaction.org" />
                  </li>
                  <li>
                    <LinkedIn url="https://www.collaction.org" />
                  </li>
                  <li>
                    <Email subject="CollAction" />
                  </li>
                </ul>
              </div>
            </div>
          </Section>
        </Layout>
      );
    }}
  />
);
