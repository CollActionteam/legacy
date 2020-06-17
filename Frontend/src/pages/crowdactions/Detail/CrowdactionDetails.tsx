import React, { useState } from 'react';
import { useMutation, useQuery } from '@apollo/client';
import gql from 'graphql-tag';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Container, Grid } from '@material-ui/core';
import { Form, useFormik, FormikContext, Field } from 'formik';
import * as Yup from 'yup';

import { ICrowdaction, CrowdactionStatus } from '../../../api/types';
import { useUser, GET_USER } from '../../../providers/UserProvider';

import { RouteComponentProps, Redirect, useHistory } from 'react-router-dom';
import { Fragments } from '../../../api/fragments';

import { Alert } from '../../../components/Alert/Alert';
import { Button } from '../../../components/Button/Button';
import CategoryTags from '../../../components/CategoryTags/CategoryTags';
import Loader from '../../../components/Loader/Loader';
import ProgressRing from '../../../components/ProgressRing/ProgressRing';
import { Section } from '../../../components/Section/Section';

import styles from './CrowdactionDetails.module.scss';
import { CrowdactionStarter } from '../../../components/CrowdactionStarter/CrowdactionStarter';
import { Helmet } from 'react-helmet';
import Formatter from '../../../formatter';
import LazyImage from '../../../components/LazyImage/LazyImage';
import { TextField } from 'formik-material-ui';
import CrowdactionComments from '../../../components/CrowdactionComments/CrowdactionComments';

type TParams = {
  slug: string;
  crowdactionId: string;
};

const CrowdactionDetailsPage = ({
  match,
}: RouteComponentProps<TParams>): any => {
  const { slug, crowdactionId } = match.params;

  const user = useUser();
  const { data, loading } = useQuery(GET_CROWDACTION, {
    variables: { id: crowdactionId },
  });
  const crowdaction = (data?.crowdaction ?? null) as ICrowdaction | null;

  const isParticipating =
    user === undefined
      ? undefined
      : user?.participates?.some(
          (part) => part.crowdaction.id === crowdactionId
        ) ?? false;

  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const history = useHistory();

  const formik = useFormik({
    initialValues: {
      email: '',
    },
    validationSchema: Yup.object(
      user === null
        ? {
            email: Yup.string()
              .required('Please enter your e-mail address')
              .email('Please enter a valid e-mail address'),
          }
        : {
            email: Yup.string(),
          }
    ),
    onSubmit: async (_values) => {
      if (user === null) {
        await commitToCrowdactionAnonymous();
      } else {
        await commitToCrowdactionLoggedIn();
      }
      formik.setSubmitting(false);
    },
  });
  const onCommit = (data: any) => {
    const error = data.crowdaction.commit.error;
    if (error) {
      setErrorMessage(error);
      console.error(error);
    } else {
      history.push(
        `/crowdactions/${encodeURIComponent(slug)}/${crowdactionId}/thankyou`
      );
    }
  };
  const [commitToCrowdactionAnonymous] = useMutation(COMMIT_ANONYMOUS, {
    variables: {
      crowdactionId: crowdactionId,
      email: formik.values.email,
    },
    onError: (data) => {
      console.error(data.message);
      setErrorMessage(data.message);
    },
    onCompleted: onCommit,
  });
  const [commitToCrowdactionLoggedIn] = useMutation(COMMIT_LOGGED_IN, {
    variables: { crowdactionId: crowdactionId },
    onCompleted: onCommit,
    onError: (data) => {
      console.error(data.message);
      setErrorMessage(data.message);
    },
    refetchQueries: [
      {
        query: GET_USER,
      },
    ],
  });

  const renderStats = (crowdaction: ICrowdaction) => {
    if (crowdaction && crowdaction.remainingTime) {
      const endDate = new Date(crowdaction.end);
      return (
        <div>
          <div className={styles.remainingTime}>
            <FontAwesomeIcon icon="clock"></FontAwesomeIcon>
            <span>{crowdaction?.remainingTimeUserFriendly}</span>
          </div>
          <div className={styles.joinButton}>
            <Button
              onClick={() => {
                const join = document.getElementById('join');
                if (join) {
                  join.scrollIntoView({ behavior: 'smooth' });
                }
              }}
            >
              Join
            </Button>
          </div>
          <div className={styles.deadline}>
            <span>
              This crowdaction will only start if it reaches its goal by
              <br></br>
              {Formatter.time(endDate)} on {Formatter.date(endDate)} (
              {Formatter.timezone()} timezone).
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

  const renderCrowdaction = (crowdaction: ICrowdaction) => {
    const description = {
      __html: crowdaction.description,
    };

    const goal = {
      __html: crowdaction.goal,
    };

    const comments = {
      __html: crowdaction.creatorComments,
    };

    const defaultBanner = require(`../../../assets/default_banner_images/${
      crowdaction.categories[0] ? crowdaction.categories[0].category : 'OTHER'
    }.jpg`);

    return (
      <>
        <Helmet>
          <title>Crowdaction {crowdaction.name}</title>
          <meta
            name="description"
            content={`Crowdaction ${crowdaction.name}`}
          />
        </Helmet>
        <Section center className={styles.title} title={crowdaction.name}>
          <p>{crowdaction.proposal}</p>
          <CategoryTags categories={crowdaction.categories}></CategoryTags>
        </Section>
        <Section className={styles.banner}>
          <Grid container>
            <Grid item md={7} xs={12}>
              <Container className={styles.bannerImage}>
                <figure className={styles.image}>
                  {crowdaction.bannerImage ? (
                    <LazyImage
                      src={crowdaction.bannerImage.url}
                      alt={crowdaction.name}
                    />
                  ) : (
                    <LazyImage src={defaultBanner} alt={crowdaction.name} />
                  )}
                </figure>
              </Container>
            </Grid>
            <Grid item md={5} xs={12}>
              <Container className={styles.stats}>
                <div className={styles.percentage}>
                  <ProgressRing
                    progress={crowdaction.percentage}
                    radius={60}
                    fontSize="var(--font-size-md)"
                  />
                </div>
                <div className={styles.count}>
                  <span>
                    {Formatter.largeNumber(crowdaction.totalParticipants)} of{' '}
                    {Formatter.largeNumber(crowdaction.target)} signups
                  </span>
                </div>
                {renderStats(crowdaction)}
              </Container>
            </Grid>
          </Grid>
        </Section>
        <Section className={styles.infoSection}>
          <Grid container>
            <Grid item md={7} xs={12}>
              <Container>
                <div>
                  <h3 className={styles.header}>Description</h3>
                  <p dangerouslySetInnerHTML={description}></p>

                  <h3 className={styles.header}>Goal</h3>
                  <p dangerouslySetInnerHTML={goal}></p>
                </div>
                {crowdaction.descriptiveImage && (
                  <div>
                    <figure className={styles.image}>
                      <LazyImage
                        src={crowdaction.descriptiveImage.url}
                        alt={crowdaction.descriptiveImage.description}
                      />
                      <p>{crowdaction.descriptiveImage.description}</p>
                    </figure>
                  </div>
                )}
                {comments && (
                  <div>
                    <h3 className={styles.header}>Other comments</h3>
                    <p dangerouslySetInnerHTML={comments}></p>
                  </div>
                )}

                {crowdaction.descriptionVideoLink && (
                  <div className={styles.video}>
                    <iframe
                      width="560"
                      height="315"
                      src={crowdaction.descriptionVideoLink}
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
                {crowdaction.owner && (
                  <CrowdactionStarter
                    user={crowdaction.owner}
                  ></CrowdactionStarter>
                )}
                {user !== undefined && isParticipating !== undefined && (
                  <div id="join" className={styles.joinSection}>
                    {crowdaction.isComingSoon && (
                      <span>
                        This crowdaction starts on{' '}
                        {Formatter.date(new Date(crowdaction.start))}.
                      </span>
                    )}
                    {crowdaction.isSuccessfull && (
                      <span>
                        This crowdaction is already done and has completed
                        successfully.
                      </span>
                    )}
                    {crowdaction.isFailed && (
                      <span>
                        This crowdaction is already done and has failed.
                      </span>
                    )}
                    {crowdaction.status !== CrowdactionStatus.RUNNING && (
                      <span>
                        This crowdaction has not been approved yet and should
                        not be reachable through the website
                      </span>
                    )}
                    {crowdaction.isActive && (
                      <React.Fragment>
                        {isParticipating && (
                          <span>
                            You are already participating in this crowdaction.
                          </span>
                        )}
                        {!isParticipating && (
                          <React.Fragment>
                            <FormikContext.Provider value={formik}>
                              <Form
                                className={styles.form}
                                onSubmit={formik.handleSubmit}
                              >
                                {user !== null && (
                                  <React.Fragment>
                                    <span className={styles.formRow}>
                                      Want to participate? Join this
                                      crowdaction!
                                    </span>
                                    <input
                                      type="hidden"
                                      name="email"
                                      {...formik.getFieldProps('email')}
                                    />
                                  </React.Fragment>
                                )}
                                {user === null && (
                                  <React.Fragment>
                                    <span>
                                      Want to participate? Enter your e-mail
                                      address and join this crowdaction!
                                    </span>
                                    <div className={styles.formRow}>
                                      <Field
                                        name="email"
                                        label="Your e-mail address"
                                        component={TextField}
                                        fullWidth
                                      ></Field>
                                    </div>
                                  </React.Fragment>
                                )}
                                <Button
                                  type="submit"
                                  disabled={formik.isSubmitting}
                                >
                                  Join
                                </Button>
                              </Form>
                            </FormikContext.Provider>
                          </React.Fragment>
                        )}
                      </React.Fragment>
                    )}
                  </div>
                )}
              </Container>
            </Grid>

            <Grid item md={7} xs={12}>
              <Container>
                <h3 className={styles.header}>User Comments</h3>
                <CrowdactionComments id={crowdactionId} />
              </Container>
            </Grid>
          </Grid>
        </Section>
      </>
    );
  };

  return (
    <>
      <Alert type="error" text={errorMessage} />
      {!loading && !data ? <Redirect to="/404" /> : null}
      {loading ? <Loader /> : null}
      {crowdaction ? renderCrowdaction(crowdaction) : null}
    </>
  );
};

const GET_CROWDACTION = gql`
  query GetCrowdaction($id: ID) {
    crowdaction(id: $id) {
      ${Fragments.crowdactionDetail}
    }
  }
`;

const COMMIT_ANONYMOUS = gql`
  mutation CommitAnonymous($crowdactionId: ID!, $email: String!) {
    crowdaction {
      commit: commitToCrowdactionAnonymous(
        crowdactionId: $crowdactionId
        email: $email
      ) {
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
  mutation CommitLoggedIn($crowdactionId: ID!) {
    crowdaction {
      commit: commitToCrowdactionLoggedIn(crowdactionId: $crowdactionId) {
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

export default CrowdactionDetailsPage;
