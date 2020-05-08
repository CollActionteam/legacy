import React from "react";
import { Section } from "../../../components/Section/Section";

import styles from "./ThankYouCreate.module.scss";
import { RouteComponentProps, Redirect } from "react-router-dom";
import { gql, useQuery } from "@apollo/client";
import Loader from "../../../components/Loader/Loader";
import {Banner} from "../../../components/Banner/Banner";
import {Grid} from "@material-ui/core";
import {Helmet} from "react-helmet";

type TParams = {
  crowdactionId: string
}

const GET_CROWDACTION = gql`
  query GetCrowdaction($id: ID) {
    crowdaction(id: $id) {
      name
    }
  }
`;

const renderThankYou = (name: string) => {

  return <>
      <Helmet>
          <title>Create a crowdaction</title>
          <meta name="description" content="Create a crowdaction" />
      </Helmet>
      <Banner>
          <Grid container className={styles.banner}>
              <Section>
                  <h1 className={styles.thankYouOverlayTitle}>Awesome!</h1>
                  <h2 className={styles.thankYouOverlaySubtitle}>
                      Thank you for submitting {name}!
                  </h2>
                  <p>
                      The CollAction Team will review your crowdaction as soon as possible.
                      <br />
                      If it meets all the CollAction criteria we’ll publish the crowdaction on
                      the website and will let you know, so you can start promoting it!
                      <br />
                      If we have any additional questions or comments, we’ll reach out to
                      you by email.
                      <br />
                      Thanks!
                  </p>
              </Section>
          </Grid>
      </Banner>
      </>;
}

const ThankYouCreatePage = ({ match } : RouteComponentProps<TParams>) => {
  const { crowdactionId } = match.params;
  const { data, loading } = useQuery(GET_CROWDACTION, { variables: { id: crowdactionId } });
  const crowdactionName = (data?.crowdaction.name ?? null) as string;

  return (
    <>
      { !loading && !data ? <Redirect to="/404" /> : null }
      { loading ? <Loader /> : null }
      { renderThankYou(crowdactionName) }
    </>
  )
};

export default ThankYouCreatePage;
