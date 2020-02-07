import React from "react";
import { Router } from "@reach/router";
import Layout from "../components/Layout";
import gql from "graphql-tag";
import Loader from "../components/Loader";
import { useQuery } from "react-apollo";
import { navigate } from "gatsby";
import { Section } from "../components/Section";

import styles from "./style.module.scss";
import CategoryTags from "../components/CategoryTags";
import { IProject } from "../api/types";
import { Grid, Container, Avatar } from "@material-ui/core";
import ProgressRing from "../components/ProgressRing";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Button } from "../components/Button/Button";

const ProjectDetails = () => {
  return (
    <Router>
      <ProjectDetailsPage path="projects/:slug/:projectId"></ProjectDetailsPage>
    </Router>
  );
};

const ProjectDetailsPage = ({ projectId }) => {
  const query = useQuery(GET_PROJECT, { variables: { id: projectId } });

  const { data, loading } = query;

  if (loading) {
    return <Loader />;
  }

  if (!data) {
    navigate("/404");
  }

  const project = data.project as IProject;

  const description = {
    __html: project.description,
  };

  const goal = {
    __html: project.goal,
  };

  const comments = {
    __html: project.creatorComments,
  };

  const banner = project.bannerImage
    ? project.bannerImage.url
    : `/assets/default_banners/${project.categories[0].category}.jpg`;

  const renderStats = () => {
    const endDate = new Date(project.end);

    if (project.remainingTime) {
      return (
        <div>
          <div className={styles.remainingTime}>
            <FontAwesomeIcon icon="clock"></FontAwesomeIcon>
            <span>{Math.round(project.remainingTime / 3600 / 24)} days</span>
          </div>
          <div className={styles.join}>
            <Button onClick={join}>Join crowdaction</Button>
          </div>
          <div className={styles.deadline}>
            <span>
              This crowdaction will only start if it reaches its goal by
              <br></br>
              {endDate.toDateString()} {endDate.toTimeString()}.
            </span>
          </div>
        </div>
      );
    } else {
      return (
        <div className={styles.remainingTime}>
          <span>Not active</span>
        </div>
      );
    }
  };

  const join = () => {
    alert("Join!");
  };

  return (
    <Layout>
      <Section center className={styles.title} title={project.name}>
        <p>{project.proposal}</p>
        <CategoryTags categories={project.categories}></CategoryTags>
      </Section>
      <Section color="grey">
        <Grid container>
          <Grid item md={7} xs={12}>
            <Container>
              <figure className={styles.image}>
                <img src={banner} alt={project.name}></img>
              </figure>
            </Container>
          </Grid>
          <Grid item md={5} xs={12}>
            <Container className={styles.stats}>
              <div className={styles.percentage}>
                <ProgressRing
                  progress={project.percentage}
                  radius={60}
                  fontSize="var(--font-size-md)"
                />
              </div>
              <div className={styles.count}>
                <span>
                  {project.totalParticipants} of {project.target} signups
                </span>
              </div>
              {renderStats()}
            </Container>
          </Grid>
        </Grid>
      </Section>
      <Section>
        <Grid container>
          <Grid item md={7} xs={12}>
            <Container>
              <div>
                <h3 className={styles.header}>Description</h3>
                <p dangerouslySetInnerHTML={description}></p>

                <h3 className={styles.header}>Goal</h3>
                <p dangerouslySetInnerHTML={goal}></p>
              </div>
              {project.descriptiveImage && (
                <div>
                  <figure className={styles.image}>
                    <img src={project.descriptiveImage.url}></img>
                    <p>{project.descriptiveImage.description}</p>
                  </figure>
                </div>
              )}
              <div>
                <h3 className={styles.header}>Other comments</h3>
                <p dangerouslySetInnerHTML={comments}></p>
              </div>

              {project.descriptionVideoLink && (
                <div className={styles.video}>
                  <iframe
                    width="560"
                    height="315"
                    src={project.descriptionVideoLink}
                    allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
                    frameBorder="0"
                    allowFullScreen
                  ></iframe>
                </div>
              )}
            </Container>
          </Grid>

          <Grid item md={5} xs={12}>
            <Container>
              <div className={styles.projectStarter}>
                <div className={styles.avatarContainer}>
                  <Avatar className={styles.avatar}>EW</Avatar>
                </div>
                <h4>{project.owner.fullName}</h4>
                <p className={styles.projectStarterTitle}>Project starter</p>
              </div>
            </Container>
          </Grid>
        </Grid>
      </Section>

      {JSON.stringify(data)}
    </Layout>
  );
};

const GET_PROJECT = gql`
  query GetProject($id: ID) {
    project(id: $id) {
      id
      name
      description
      categories {
        category
      }
      bannerImage {
        url
      }
      descriptiveImage {
        url
        description
      }
      goal
      end
      target
      proposal
      creatorComments
      descriptionVideoLink
      owner {
        fullName
      }
      remainingTime
      totalParticipants
      percentage
      isActive
      isComingSoon
      isClosed
      isSuccessfull
      isFailed
    }
  }
`;

export default ProjectDetails;
