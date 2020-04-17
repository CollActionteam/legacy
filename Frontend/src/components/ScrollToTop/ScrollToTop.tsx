import { useEffect } from "react";
import { useLocation } from "react-router-dom";

// When we click a link, with react router the page stays at the current scroll location
// To restore navigation, this component will scroll to the top on history navigate
// See: https://reacttraining.com/react-router/web/guides/scroll-restoration
export default function ScrollToTop() {
  const { pathname } = useLocation();

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [pathname]);

  return null;
}
