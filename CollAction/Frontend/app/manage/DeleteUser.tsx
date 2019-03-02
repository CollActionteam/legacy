import * as React from "react";
import Collapsible from "react-collapsible";

import renderComponentIf from "../global/renderComponentIf";


export default class DeleteUser extends React.Component {
  render() {
    return (
      <Collapsible
        transitionTime={300}
        trigger={<a href="javascript:void(0)" className="btn">Remove my account</a>}
        triggerWhenOpen={<a href="javascript:void(0)" className="btn">I think I'll stay</a>}>
        <p>
          Are you sure you want to delete your account from CollAction.org? Without the account,
          you will not be able to participate in projects on CollAction. Deleting your account
          will not unsubscribe you from the newsletter - to do that, simply click "unsubscribe"
          at the bottom of one of the newsletters (you can also do this without deleting the account).
        </p>
        <p>
          Don't worry - if you have started any projects in the past, they will remain visible.
          Please reach out to us at <a href="mailto:collactionteam@gmail.com">collactionteam@gmail.com</a>
          if you would like to remove them.
        </p>
        <input type="submit" value="Yes, remove me." />
      </Collapsible>
    );
  }
}

renderComponentIf(
    <DeleteUser></DeleteUser>,
    document.getElementById("delete-user")
);

