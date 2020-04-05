import { Grid, Card, CardActions } from "@material-ui/core";
import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Link } from "react-router-dom";
import { useUser } from "../../../providers/User";
import ResetPassword from "../../../components/Profile/ResetPassword";
import RecurringDonations from "../../../components/Profile/RecurringDonations";
import ProjectsParticipating from "../../../components/Profile/ProjectsParticipating";
import NewsletterSubscription from "../../../components/Profile/NewsletterSubscription";
import DeleteAccount from "../../../components/Profile/DeleteAccount";
import ProjectsCreated from "../../../components/Profile/ProjectsCreated";

export default () => {
  const { user } = useUser();
  if (user !== null) {
    return <div style={{ padding: 20 }}>
        <Grid container spacing={5}>
        {
          user.isAdmin ?
            <React.Fragment>
              <Grid item xs={12} md={6}>
                <Card>
                  <CardActions>
                    <Link to="/admin/projects/list">
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
            </React.Fragment> :
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
          <ProjectsParticipating user={user} />
        </Grid>
        <Grid item xs={12} md={6}>
          <ProjectsCreated user={user} />
        </Grid>
      </Grid>
    </div>;
  } else {
    return <h1>Please log in before viewing your profile</h1>;
  }
};