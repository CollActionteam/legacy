import React from 'react'
import {Grid} from "@material-ui/core";
import {Section} from "../../components/Section/Section";
import {Helmet} from "react-helmet";

const NotFoundPage = () => <>
    <Helmet>
        <title>Page not found</title>
        <meta name="description" content="Page not found"/>
    </Helmet>
    <Grid container>
        <Section>
            <h1>Not found...</h1>
            <p>
                Unfortunately the page could not be found. <a href="/">Return to the homepage</a>.
            </p>
        </Section>
    </Grid>
</>;

export default NotFoundPage;