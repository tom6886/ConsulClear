package tb.consul.clear.web.controller;

import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPut;
import org.apache.http.impl.client.HttpClientBuilder;
import org.apache.http.util.EntityUtils;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.servlet.ModelAndView;
import tb.consul.clear.web.util.R;

import java.io.IOException;
import java.util.List;

/**
 * @author tangbo
 */
@Controller
@RequestMapping("/")
public class IndexController {
    @RequestMapping("")
    public ModelAndView index() {
        return new ModelAndView("index");
    }

    @RequestMapping(value = "/getData/{address}"
            , method = RequestMethod.POST
            , produces = "application/json; charset=utf-8")
    @ResponseBody
    public R getCritical(@PathVariable String address) {
        HttpClient client = HttpClientBuilder.create().build();
        String queryAddr = "/v1/health/state/critical";
        HttpGet get = new HttpGet("http://" + address + queryAddr);
        try {
            HttpResponse response = client.execute(get);
            String result = EntityUtils.toString(response.getEntity());
            return R.ok(result);
        } catch (IOException e) {
            e.printStackTrace();
            return R.error(e.getMessage());
        }
    }

    @RequestMapping(value = "/clearData/{address}"
            , method = RequestMethod.POST
            , produces = "application/json; charset=utf-8")
    @ResponseBody
    public R clearCritical(@PathVariable String address, @RequestBody List<String> ids) {
        HttpClient client = HttpClientBuilder.create().build();
        int removed = 0;
        for (String id : ids) {
            String clearAddr = "/v1/agent/service/deregister/";
            HttpPut put = new HttpPut("http://" + address + clearAddr + id);
            try {
                HttpResponse response = client.execute(put);
                String result = EntityUtils.toString(response.getEntity());
                removed++;
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        return R.ok(removed);
    }
}
