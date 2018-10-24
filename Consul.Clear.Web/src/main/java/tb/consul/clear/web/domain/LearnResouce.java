package tb.consul.clear.web.domain;

import lombok.Data;

/**
 * @author tangbo
 */
@Data
public class LearnResouce {
    private String author;
    private String title;
    private String url;

    public LearnResouce(String author, String title, String url) {
        this.author = author;
        this.title = title;
        this.url = url;
    }
}
