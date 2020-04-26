import React, { useState } from "react";
import { useMutation, useQuery } from "@apollo/client";
import gql from "graphql-tag";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Container, Grid, FormControl, TextField } from "@material-ui/core";
import { Form, useFormik, FormikContext } from "formik";
import * as Yup from "yup";

import { IProject } from "../../../api/types";
import { useUser, GET_USER } from "../../../providers/UserProvider";

import { RouteComponentProps, Redirect, useHistory} from "react-router-dom";
import { Fragments } from "../../../api/fragments";

import { Alert } from "../../../components/Alert/Alert";
import { Button } from "../../../components/Button/Button";
import CategoryTags from "../../../components/CategoryTags/CategoryTags";
import Loader from "../../../components/Loader/Loader";
import ProgressRing from "../../../components/ProgressRing/ProgressRing";
import { Section } from "../../../components/Section/Section";

import styles from "./ProjectDetails.module.scss";
import DisqusProjectComments from "../../../components/DisqusProjectComments/DisqusProjectComments";
import { ProjectStarter } from "../../../components/ProjectStarter/ProjectStarter";
import Helmet from "react-helmet";

type TParams = {
  slug: string,
  projectId: string
}

const ProjectDetailsPage = ({ match } : RouteComponentProps<TParams>): any => {
  const user = useUser();
  const { slug, projectId } = match.params;
  const { data, loading } = useQuery(GET_PROJECT, { variables: { id: projectId } });
  const project = (data?.project ?? null) as IProject | null;
  const [ errorMessage, setErrorMessage ] = useState<string | null>(null);
  const isParticipating = Boolean(user?.participates?.find((part) => part.project.id === projectId));
  const history = useHistory();
  const formik = useFormik({
    initialValues: {
      email: ''
    },
    validationSchema: Yup.object(
      user === null ? {
        email: Yup.string()
          .required("Please enter your e-mail address")
          .email("Please enter a valid e-mail address"),
      } : {
        email: Yup.string() 
      }),
    onSubmit: async (_values) => { 
      if (user === null) {
          await commitToProjectAnonymous();
       } else {
          await commitToProjectLoggedIn();
       }
       formik.setSubmitting(false);
    }
  });
  const onCommit = (data: any) => {
    const error = data.project.commit.error;
    if (error) {
      setErrorMessage(error);
      console.error(error);
    } else {
      history.push(`/projects/${encodeURIComponent(slug)}/${projectId}/thankyou`);
    }
  }
  const [ commitToProjectAnonymous ] = useMutation(
    COMMIT_ANONYMOUS,
    {
      variables: { 
        projectId: projectId,
        email: formik.values.email
      },
      onError: (data: any) => setErrorMessage(data.message),
      onCompleted: onCommit
    });
  const [ commitToProjectLoggedIn ] = useMutation(
    COMMIT_LOGGED_IN,
    {
      variables: { projectId: projectId },
      onCompleted: onCommit,
      onError: (data: any) => setErrorMessage(data.message),
      refetchQueries: [{
        query: GET_USER
      }]
    });

  const renderStats = (project: IProject) => {
    if (project && project.remainingTime) {
      const endDate = new Date(project.end);
      return (
        <div>
          <div className={styles.remainingTime}>
            <FontAwesomeIcon icon="clock"></FontAwesomeIcon>
            <span>{ project?.remainingTimeUserFriendly }</span>
          </div>
          <div className={styles.joinButton}>
            <Button
              onClick={() => {
                const join = document.getElementById("join");
                if (join) {
                  join.scrollIntoView({ behavior: "smooth" });
                }
              }}
            >
              Join crowdaction
            </Button>
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

  const renderProject = (project: IProject) => {
    const description = {
      __html: project.description,
    };

    const goal = {
      __html: project.goal,
    };

    const comments = {
      __html: project.creatorComments,
    };

    const defaultBanner = require(`../../../assets/default_banners/${project.categories[0] ? project.categories[0].category : "OTHER"}.jpg`);

    return <React.Fragment>
      <Helmet>
          <title>Project { project.name }</title>
          <meta name="description" content={`Project ${project.name}`} />
      </Helmet>
      <Section center className={styles.title} title={project.name}>
        <p>{project.proposal}</p>
        <CategoryTags categories={project.categories}></CategoryTags>
      </Section>
      <Section className={styles.banner}>
        <Grid container>
          <Grid item md={7} xs={12}>
            <Container className={styles.bannerImage}>
              <figure className={styles.image}>
                { project.bannerImage ? <img src={project.bannerImage.url} alt={project.name} /> : <img src={defaultBanner} alt={project.name} /> }
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
              {renderStats(project)}
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
                    <img src={project.descriptiveImage.url} alt={project.descriptiveImage.description} />
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
                    title="video"
                  ></iframe>
                </div>
              )}
            </Container>
          </Grid>

          <Grid item md={5} xs={12}>
            <Container>
              { project.owner && 
                <ProjectStarter user={project.owner}></ProjectStarter>
              }
              
              { !isParticipating && project?.isActive ?
                  <div id="join" className={styles.joinSection}>
                    <FormikContext.Provider value={formik}>
                      <Form className={styles.form} onSubmit={formik.handleSubmit}>
                        { user === null ?
                            <React.Fragment>
                              <span>
                                Want to participate? Enter your e-mail address and join
                                this crowdaction!
                              </span>
                              <FormControl>
                                <TextField
                                  name="email"
                                  className={styles.formControl}
                                  label="Your e-mail address"
                                  type="e-mail"
                                  { ...formik.getFieldProps('email') }
                                />
                                { (formik.touched.email && formik.errors.email) ? <Alert type="error" text={formik.errors.email} /> : null }
                              </FormControl>
                            </React.Fragment> :
                            <span>
                              <input type="hidden" name="email" { ...formik.getFieldProps('email') } />
                              Want to participate? Join this crowdaction!
                            </span> 
                        }
                        <Button type="submit" disabled={formik.isSubmitting}>
                          Join CrowdAction
                        </Button>
                      </Form>
                    </FormikContext.Provider>
                  </div> : 
                    <div id="join" className={styles.joinSection}>
                      <span>
                        { project?.isActive ? "You are already participating in this project" : null }
                        { project?.isComingSoon ? `This project starts on ${new Date(project.start).toDateString()}` : null }
                        { project?.isSuccessfull ? "This project is already done and has completed successfully" : null }
                        { project?.isFailed ? "This project is already done and has failed" : null }
                      </span>
                    </div>
              }
            </Container>
          </Grid>
          <Grid item xs={12}>
            <Container>
              <DisqusProjectComments project={project} />
            </Container>
          </Grid>
        </Grid>
      </Section>
    </React.Fragment>;
  }

  return (
    <React.Fragment>
      <Alert type="error" text={errorMessage} />
      { !loading && !data ? <Redirect to="/404" /> : null }
      { loading ? <Loader /> : null }
      { project ? renderProject(project) : null }
    </React.Fragment>
  );
};

const GET_PROJECT = gql`
  query GetProject($id: ID) {
    project(id: $id) {
      ${Fragments.projectDetail}
    }
  }
`;

const COMMIT_ANONYMOUS = gql`
  mutation CommitAnonymous($projectId: ID!, $email: String!) {
    project {
      commit: commitToProjectAnonymous(projectId: $projectId, email: $email) {
        error
        loggedIn
        scenario
        userAdded
        userAlreadyActive
        userCreated
      }
    }
  }
`;

const COMMIT_LOGGED_IN = gql`
  mutation CommitLoggedIn($projectId: ID!) {
    project {
      commit: commitToProjectLoggedIn(projectId: $projectId) {
        error
        loggedIn
        scenario
        userAdded
        userAlreadyActive
        userCreated
      }
    }
  }
`;

export default ProjectDetailsPage;
