import { RouteComponentProps } from "react-router-dom";
import React from "react";
import {Grid} from "@material-ui/core";
import {Section} from "../../../components/Section/Section";
import ProjectCard from "../../../components/ProjectCard/ProjectCard";
import {gql, useQuery} from "@apollo/client";
import {IProject} from "../../../api/types";
import {Fragments} from "../../../api/fragments";
import styles from "./ThankYouCommit.module.scss";
import DonationPage from "../../Donation/Donation";

type TParams = {
  slug: string,
  projectId: string
}

const ThankYouCommitPage = ({ match } : RouteComponentProps<TParams>): any => {
  const { slug, projectId } = match.params;
  const { data, loading } = useQuery(GET_PROJECT, { variables: { id: projectId } });
  const project = (data?.project ?? null) as IProject | null;
  return <>
    <div className={styles.header}>
      <Grid container>
          <h1>Thank you for joining this project!</h1>
      </Grid>
    </div>
    <Section>
      <Grid container spacing={10}>
        <Grid item sm={12} md={6}>
          {project && <ProjectCard project={project} />}
        </Grid>
        <Grid item sm={12} md={6}>
          <h2>Now go make waves and share it!</h2>
          <br />
        </Grid>
      </Grid>
    </Section>

    <DonationPage />

  </>
}

const GET_PROJECT = gql`
  query GetProject($id: ID) {
    project(id: $id) {
      ${Fragments.projectDetail}
    }
  }
`;


export default ThankYouCommitPage;