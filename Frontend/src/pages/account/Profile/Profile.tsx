import { Grid, Card, CardActions } from "@material-ui/core";
import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Link } from "react-router-dom";
import { useUser } from "../../../providers/UserProvider";
import ResetPassword from "../../../components/Profile/ResetPassword";
import RecurringDonations from "../../../components/Profile/RecurringDonations";
import CrowdactionsParticipating from "../../../components/Profile/CrowdactionsParticipating";
import NewsletterSubscription from "../../../components/Profile/NewsletterSubscription";
import DeleteAccount from "../../../components/Profile/DeleteAccount";
import CrowdactionsCreated from "../../../components/Profile/CrowdactionsCreated";
import { Helmet } from "react-helmet";
import {Section} from "../../../components/Section/Section";

const Profile = () => {
  const user = useUser() ?? null;
  if (user !== null) {
    return <div style={{ padding: 20 }}>
        <Helmet>
          <title>User Profile</title>
          <meta name="description" content="User Profile" />
        </Helmet>
        <Grid container spacing={5}>
        {
          user.isAdmin ?
            <>
              <Grid item xs={12} md={6}>
                <Card>
                  <CardActions>
                    <Link to="/admin/crowdactions/list">
                      <FontAwesomeIcon icon="tools" />&nbsp; Manage Site
                    </Link>
                  </CardActions>
                </Card>
              </Grid>
              <Grid item xs={12} md={6}>
                <Card>
                  <CardActions>
                    <a href={`${process.env.REACT_APP_BACKEND_URL}/hangfire`}>
                      <FontAwesomeIcon icon="tools" />&nbsp; Jobs
                    </a>
                  </CardActions>
                </Card>
              </Grid>
            </> :
            null
        }
        <Grid item xs={12} md={6}>
          <ResetPassword />
        </Grid>
        <Grid item xs={12} md={6}>
          <NewsletterSubscription user={user} />
        </Grid>
        <Grid item xs={12} md={6}>
          <DeleteAccount user={user} />
        </Grid>
        <Grid item xs={12} md={6}>
          <RecurringDonations user={user} />
        </Grid>
        <Grid item xs={12} md={6}>
          <CrowdactionsParticipating user={user} />
        </Grid>
        <Grid item xs={12} md={6}>
          <CrowdactionsCreated user={user} />
        </Grid>
      </Grid>
    </div>;
  } else {
    return <Grid container>
          <Section>
              <h1>Please log in before viewing your profile</h1>
          </Section>
      </Grid>;
  }
};

export default Profile;