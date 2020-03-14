import { Grid, Card, CardActions } from "@material-ui/core";
import React from "react";
import { UserContext } from "../../providers/user";
import ResetPassword from "../../components/Profile/ResetPassword";
import NewsletterSubscription from "../../components/Profile/NewsletterSubscription";
import RecurringDonations from "../../components/Profile/RecurringDonations";
import DeleteAccount from "../../components/Profile/DeleteAccount";
import ProjectsCreated from "../../components/Profile/ProjectsCreated";
import ProjectsParticipating from "../../components/Profile/ProjectsParticipating";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Link } from "react-router-dom";

export default () => {
  return <UserContext.Consumer>
    {
      ({ user }) => {
        if (user !== null) {
          return <Grid container spacing={5}>
            <Grid item xs={6}>
              <ResetPassword />
            </Grid>
            <Grid item xs={6}>
              <NewsletterSubscription user={user!} />
            </Grid>
            <Grid item xs={6}>
              <DeleteAccount user={user!} />
            </Grid>
            <Grid item xs={6}>
              <ProjectsCreated user={user!} />
            </Grid>
            <Grid item xs={6}>
              <ProjectsParticipating user={user!} />
            </Grid>
            <Grid item xs={6}>
              <RecurringDonations user={user!} />
            </Grid>
            {
              user.isAdmin ?
                <Grid item xs={6}>
                  <Card>
                    <CardActions>
                      <Link to="/admin">
                        <FontAwesomeIcon icon="tools" />
                        Manage Site
                      </Link>
                    </CardActions>
                  </Card>
                </Grid> :
                null
            }
          </Grid>;
        } else {
          return <h1>Please log in before viewing your profile</h1>;
        }
      }
    }
  </UserContext.Consumer>;
};