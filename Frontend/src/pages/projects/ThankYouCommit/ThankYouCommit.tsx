import {RouteComponentProps} from "react-router-dom";
import React from "react";
import {Grid, Hidden} from "@material-ui/core";
import {Section} from "../../../components/Section/Section";
import ProjectCard from "../../../components/ProjectCard/ProjectCard";
import {gql, useQuery} from "@apollo/client";
import {IProject} from "../../../api/types";
import {Fragments} from "../../../api/fragments";
import styles from "./ThankYouCommit.module.scss";
import ProjectShare from "../../../components/ProjectShare/ProjectShare";
import DonationLayout from "../../../components/DonationLayout/DonationLayout";

type TParams = {
    slug: string,
    projectId: string
}

const ThankYouCommitPage = ({match}: RouteComponentProps<TParams>): any => {
    const {projectId} = match.params;
    const {data} = useQuery(GET_PROJECT, {variables: {id: projectId}});
    const project = (data?.project ?? null) as IProject | null;
    return <>
        <Section className={styles.header}>
            <Grid container spacing={10}>
                <Hidden smDown>
                    <Grid item md={5}/>
                </Hidden>
                <Grid item sm={12} md={7}>
                    <h2>Thank you for joining this project!</h2>
                </Grid>
            </Grid>
        </Section>
        <Section className={styles.section}>
            <Grid container spacing={10}>
                <Hidden smDown>
                    <Grid item md={5} className={styles.projectGridItem}>
                        {project && <ProjectCard project={project}/>}
                    </Grid>
                </Hidden>
                <Grid item sm={12} md={7}>
                    <h3>Now go make waves and share it!</h3>

                    {project && <ProjectShare project={project} />}
                    <br/>
                </Grid>
            </Grid>
        </Section>

        <DonationLayout />

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