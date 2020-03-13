import React from "react";
import { useMutation, useQuery } from "@apollo/client";
import gql from "graphql-tag";
import { ExecutionResult } from "graphql";
import { IProject } from "../../../api/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Avatar, Container, Grid, FormControl } from "@material-ui/core";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";
import { TextField } from "formik-material-ui";

import { Button } from "../../../components/Button/Button";
import CategoryTags from "../../../components/CategoryTags";
import Loader from "../../../components/Loader";
import ProgressRing from "../../../components/ProgressRing";
import { Section } from "../../../components/Section";

import styles from "./ProjectDetails.module.scss";
import { RouteComponentProps, Redirect} from "react-router-dom";

type TParams = {
  slug: string,
  projectId: string
}

const ProjectDetailsPage = ({ match } : RouteComponentProps<TParams>): any => {
  const projectId = match.params.projectId;

  const query = useQuery(GET_PROJECT, { variables: { id: projectId } });
  const { data, loading } = query;

  const [commitToProject] = useMutation(COMMIT_ANONYMOUS);

  const commit = async (form: any) => {
    const response = (await commitToProject({
      variables: {
        projectId: project.id,
        email: form.participantEmail,
      },
    })) as ExecutionResult;
    console.log(response);
  };

  if (loading) {
    return <Loader />;
  }

  if (!data) {
    return <Redirect to="/404"></Redirect>
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

  return (
    <React.Fragment>
      <Section center className={styles.title} title={project.name}>
        <p>{project.proposal}</p>
        <CategoryTags categories={project.categories}></CategoryTags>
      </Section>
      <Section className={styles.banner}>
        <Grid container>
          <Grid item md={7} xs={12}>
            <Container className={styles.bannerImage}>
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
              <div className={styles.projectStarter}>
                <div className={styles.avatarContainer}>
                  <Avatar className={styles.avatar}>
                    {project.owner.firstName.charAt(0)}
                    {project.owner.lastName.charAt(0)}
                  </Avatar>
                </div>
                <h4>{project.owner.fullName}</h4>
                <p className={styles.projectStarterTitle}>Project starter</p>
              </div>
              <div id="join" className={styles.joinSection}>
                <Formik
                  initialValues={{
                    participantEmail: "",
                  }}
                  validateOnChange={false}
                  validateOnBlur={true}
                  validateOnMount={false}
                  validationSchema={Yup.object({
                    participantEmail: Yup.string()
                      .required("Please enter your e-mail address")
                      .email("Please enter a valid e-mail address"),
                  })}
                  onSubmit={async values => {
                    await commit(values);
                  }}
                >
                  {props => (
                    <Form className={styles.form}>
                      <span>
                        Want to participate? Enter your e-mail address and join
                        this crowdaction!
                      </span>
                      <FormControl>
                        <Field
                          name="participantEmail"
                          label="Your e-mail address"
                          component={TextField}
                          fullWidth
                        ></Field>
                      </FormControl>
                      <Button type="submit" disabled={props.isSubmitting}>
                        Join CrowdAction
                      </Button>
                    </Form>
                  )}
                </Formik>
              </div>
            </Container>
          </Grid>
        </Grid>
      </Section>
    </React.Fragment>
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
        firstName
        lastName
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

const COMMIT_ANONYMOUS = gql`
  mutation Commit($projectId: Int!, $email: String!) {
    project {
      commitToProjectAnonymous(projectId: $projectId, email: $email) {
        error
        userAdded
        userAlreadyActive
        userCreated
      }
    }
  }
`;

export default ProjectDetailsPage;
